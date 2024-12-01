using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using ReactiveUI;
using Splat;
using Syncfusion.UI.Xaml.TreeView;
using WolvenKit.App.Interaction;
using WolvenKit.App.Services;
using WolvenKit.App.ViewModels.Documents;
using WolvenKit.App.ViewModels.Shell;
using WolvenKit.Common.Services;
using WolvenKit.Core.Interfaces;
using WolvenKit.Core.Services;
using WolvenKit.RED4.Types;
using WolvenKit.Views.Dialogs.Windows;

namespace WolvenKit.Views.Tools
{
    /// <summary>
    /// Interaction logic for RedTreeView.xaml
    /// </summary>
    public partial class RedTreeView : UserControl
    {
        private readonly IModifierViewStateService _modifierViewStateSvc;
        private readonly ILoggerService _loggerService;
        private readonly IProgressService<double> _progressService;

        public RedTreeView()
        {
            _modifierViewStateSvc = Locator.Current.GetService<IModifierViewStateService>();
            _loggerService = Locator.Current.GetService<ILoggerService>();
            _progressService = Locator.Current.GetService<IProgressService<double>>(); 
            
            InitializeComponent();

            DataContextChanged += OnDataContextChanged;

            // Listen for the "UpdateFilteredItemsSource" message
            MessageBus.Current.Listen<string>("Command")
                .Where(x => x == "UpdateFilteredItemsSource")
                .Subscribe(_ => UpdateFilteredItemsSource(ItemsSource));

            TreeView.ApplyTemplate();
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is INotifyPropertyChanged oldViewModel)
            {
                oldViewModel.PropertyChanged -= OnViewModelPropertyChanged;
            }

            if (e.NewValue is INotifyPropertyChanged newViewModel)
            {
                newViewModel.PropertyChanged += OnViewModelPropertyChanged;
            }
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is RDTDataViewModel dtm && e.PropertyName == nameof(dtm.CurrentSearch))
            {
                UpdateFilteredItemsSource(dtm.Chunks);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void UpdateFilteredItemsSource(object value)
        {
            var collectionView = value switch
            {
                IEnumerable<ChunkViewModel> itemsSource => CollectionViewSource.GetDefaultView(itemsSource),
                ICollectionView view => view,
                _ => null
            };

            if (collectionView is not null)
            {
                collectionView.Filter = item =>
                    (item as ChunkViewModel)?.IsHiddenByEditorDifficultyLevel != true &&
                    (item as ChunkViewModel)?.IsHiddenBySearch != true;
                SetCurrentValue(ItemsSourceProperty, collectionView);
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ItemsSource)));
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(RedTreeView),
                new PropertyMetadata(null, OnItemsSourceChanged));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RedTreeView redTreeView)
            {
                redTreeView.UpdateFilteredItemsSource(e.NewValue);
            }
        }
        
        public object ItemsSource
        {
            get => GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        /// <summary>Identifies the <see cref="SelectedItem"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(RedTreeView));

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(nameof(SelectedItems), typeof(object), typeof(RedTreeView));

        public object SelectedItems
        {
            get => GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        private void OnSelectionChanged(object sender, ItemSelectionChangedEventArgs e)
        {
            foreach (var removedItem in e.RemovedItems.OfType<ISelectableTreeViewItemModel>())
            {
                removedItem.IsSelected = false;
            }

            foreach (var addedItem in e.AddedItems.OfType<ISelectableTreeViewItemModel>())
            {
                addedItem.IsSelected = true;
            }

            RefreshSelectedItemsContextMenuFlags();
            RefreshCommandStatus();
        }


        private void OnExpanded(object sender, NodeExpandedCollapsedEventArgs e)
        {
            if (!ModifierViewStateService.IsShiftBeingHeld || e.Node.Content is not ChunkViewModel chunk)
            {
                return;
            }

            chunk.SetChildExpansionStates(true);
        }

        private async void OnCollapsed(object sender, NodeExpandedCollapsedEventArgs e)
        {
            if (e.Node.Content is not ChunkViewModel cvm)
            {
                return;
            }

            if (ModifierViewStateService.IsShiftBeingHeld)
            {
                cvm.SetChildExpansionStates(false);
            }

            if (e.Node.Level != 0 || cvm.ResolvedData is not worldStreamingSector)
            {
                return;
            }

            var tt = Locator.Current.GetService<AppViewModel>();

            if (tt.ActiveDocument is not RedDocumentViewModel { SelectedTabItemViewModel: RDTDataViewModel rdtd })
            {
                return;
            }

            var chunk = rdtd.Chunks.First();
            try
            {
                await chunk.Refresh();
            }
            catch (Exception ex) { _loggerService.Error(ex); }
        }


        // Drag & Drop Functionality

        //private string dropFileLocation;
        //private RedTypeDto dropFile;

        private void SfTreeView_ItemDragStarting(object sender, TreeViewItemDragStartingEventArgs e)
        {
            //var record = e.DraggingNodes[0].Content as ChunkViewModel;
            //if (typeof(CBool).IsAssignableTo(record.Data.GetType()))
            //    e.Cancel = true;
        }

        public bool IsControlBeingHeld
        {
            get => (bool)GetValue(IsControlBeingHeldProperty);
            set => SetValue(IsControlBeingHeldProperty, value);
        }

        public static readonly DependencyProperty IsControlBeingHeldProperty =
            DependencyProperty.Register(nameof(IsControlBeingHeld), typeof(bool), typeof(RedTreeView));

        private bool IsAllowDrop(TreeViewItemDragOverEventArgs e)
        {
            if (e.DraggingNodes?[0].Content is not ChunkViewModel source ||
                e.TargetNode?.Content is not ChunkViewModel target ||
                source == target || target.Parent == null || target.Parent.IsReadOnly)
            {
                return false;
            }


            switch (IsControlBeingHeld)
            {
                case false when !target.IsInArray:
                case true when target.Parent.Data is IRedBufferPointer:
                    return true;
                case true:
                {
                    if (target.Parent.Data is not IRedArray arr)
                    {
                        return false;
                    }

                    var arrayType = target.Parent.Data.GetType().GetGenericTypeDefinition();

                    return arrayType == typeof(CArray<>) || (arrayType == typeof(CStatic<>) && arr.Count < arr.MaxSize);
                }
                default:
                    return true;
            }
        }

        private void SfTreeView_ItemDragOver(object sender, TreeViewItemDragOverEventArgs e)
        {
            if (sender is not SfTreeView)
            {
                return;
            }

            if (e.DropPosition is DropPosition.DropAsChild or DropPosition.DropHere)
            {
                e.DropPosition = DropPosition.DropBelow;
            }

            e.DropPosition = IsAllowDrop(e) ? e.DropPosition : DropPosition.None;

        }

        private async void SfTreeView_ItemDropping(object sender, TreeViewItemDroppingEventArgs e)
        {
            if (e.DraggingNodes == null
                || e.TargetNode.Content is not ChunkViewModel target
                || target.Parent == null
                || (e.DropPosition != DropPosition.DropBelow && e.DropPosition != DropPosition.DropAbove))
            {
                e.Handled = true;
                return;
            }

            var indexOffset = e.DropPosition == DropPosition.DropBelow ? 1 : 0;
            var targetIndex = target.Parent.Properties.IndexOf(target) + indexOffset;

            foreach (var node in e.DraggingNodes.Where(node => node.Content is ChunkViewModel))
            {
                var source = (ChunkViewModel)node.Content;

                if (IsControlBeingHeld && source.Data is IRedCloneable irc)
                {
                    if (await Interactions.ShowMessageBoxAsync($"Duplicate {source.Data.GetType().Name} here?",
                            "Duplicate Confirmation", WMessageBoxButtons.YesNo) == WMessageBoxResult.Yes)
                    {
                        target.Parent.InsertChild(targetIndex, (IRedType)irc.DeepCopy());
                    }
                }
                else
                {
                    Debug.WriteLine("Moved a node to " + targetIndex);
                    target.Parent.MoveChild(targetIndex, source);
                }
            }

            e.Handled = true;
        }

        private void SfTreeView_ItemDropped(object sender, TreeViewItemDroppedEventArgs e)
        {
            //if (e.DraggingNodes != null && e.DraggingNodes[0].Content is ChunkViewModel source)
            //{
            //    if (e.TargetNode.Content is ChunkViewModel target)
            //    {
            //        if (source.Data is RedBaseClass rbc && target.Parent.Data is DataBuffer)
            //        {
            //            target.Parent.AddChunkToDataBuffer((RedBaseClass)rbc.DeepCopy(), target.Parent.Properties.IndexOf(target) + 1);
            //        }
            //    }
            //}

            //if (e.Data.GetDataPresent(DataFormats.FileDrop))
            //{
            //    if (e.TargetNode.Content is ChunkViewModel target)
            //    {
            //        var files = new List<string>((string[])e.Data.GetData(DataFormats.FileDrop));
            //        if (files.Count == 1)
            //        {
            //            if (dropFileLocation == files[0])
            //            {
            //                //target.Data = dropFile.Data;
            //            }
            //        }
            //    }
            //}
        }

        private void Copy_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (sender is not Grid g || g.DataContext is not ChunkViewModel cvm || cvm.Value == "null")
            {
                return;
            }

            e.CanExecute = true;
            e.Handled = true;

        }

        private void Copy_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is not Grid { DataContext: ChunkViewModel cvm })
            {
                return;
            }

            if (cvm.Data is TweakDBID tweakDbId)
            {
                Clipboard.SetText(
                    (tweakDbId.IsResolvable ? tweakDbId.ResolvedText : ((ulong)tweakDbId).ToString("X16")) ??
                    string.Empty);
                return;
            }

            if (cvm.Value != null)
            {
                Clipboard.SetText(cvm.Value);
            }
        }

        #region commands

        private void RefreshCommandStatus() => DuplicateSelectionCommand.NotifyCanExecuteChanged();


        private async Task DuplicateSelectedChunks(bool preserveIndex = false)
        {
            var chunks = GetSelectedChunks();


            var tasks = chunks.Select(cvm => cvm.DuplicateChunkAsync(preserveIndex ? -1 : cvm.NodeIdxInParent + chunks.Count)).ToList();

            var duplicatedChunks = await Task.WhenAll(tasks);

            var newChunks = duplicatedChunks.Where(newChunk => newChunk != null).ToList();


            if (ItemsSource is not ICollectionView collectionView)
            {
                return;
            }

            using (collectionView.DeferRefresh())
            {
                foreach (var cvm in chunks)
                {
                    cvm.IsSelected = false;
                }

                foreach (var cvm in newChunks)
                {
                    cvm.IsSelected = true;
                }
            }
        }

        /// <summary>
        /// Duplicates each chunk in selection, appending new changes after the selected items.
        /// </summary>
        [RelayCommand]
        private async Task DuplicateSelection() => await DuplicateSelectedChunks(false);

        /// <summary>
        /// Duplicates each chunk in selection, adding every new chunk directly next to the original. 
        /// </summary>
        [RelayCommand]
        private async Task DuplicateSelectionInPlace() => await DuplicateSelectedChunks(true);

        [RelayCommand]
        private async Task DuplicateSelectionAsNew()
        {
            var chunks = GetSelectedChunks();

            foreach (var cvm in chunks)
            {
                await cvm.DuplicateChunkAsNewAsync();
            }
        }


        private bool CanOpenSearchAndReplaceDialog => !_isContextMenuOpen &&
                                                      SelectedItem is ChunkViewModel { Parent: not null } &&
                                                      !SearchAndReplaceDialog.IsInstanceOpen;

        // [RelayCommand(CanExecute = nameof(CanOpenSearchAndReplaceDialog))] this doesn't work if item is called from menu
        [RelayCommand]
        private void OpenSearchAndReplaceDialog()
        {
            var selectedChunkViewModels = GetSelectedChunks();
            if (selectedChunkViewModels.Count == 0 || SelectedItem is not ChunkViewModel cvm)
            {
                return;
            }

            // cvm.PropertyChanged += OnCvmExpansionStateChange;

            _progressService.IsIndeterminate = true;
            
            var dialog = new SearchAndReplaceDialog();
            if (dialog.ShowDialog() != true)
            {
                _progressService.IsIndeterminate = false;
                return;
            }

            var searchText = dialog.ViewModel?.SearchText ?? "";
            var replaceText = dialog.ViewModel?.ReplaceText ?? "";
            var isRegex = dialog.ViewModel?.IsRegex ?? false;
            var isWholeWord = dialog.ViewModel?.IsWholeWord ?? false;

            ChunkViewModel.SearchAndReplace_ResetCaches();
            
            var results = selectedChunkViewModels
                .Select(item => item.SearchAndReplace(searchText, replaceText, isWholeWord, isRegex))
                .ToList();

            var numReplaced = results.Sum();

            _loggerService.Info($"Replaced {numReplaced} occurrences of '{searchText}' with '{replaceText}'");
            
            if (numReplaced <= 0)
            {
                _progressService.IsIndeterminate = false;
                return;
            }

            GetRoot().Tab?.Parent.SetIsDirty(true);

            _progressService.IsIndeterminate = false;
        }

        [RelayCommand]
        private void DeleteSelection()
        {
            if (ItemsSource is not ICollectionView collectionView)
            {
                return;
            }

            using (collectionView.DeferRefresh())
            {
                foreach (var chunkSiblings in GetSelectedChunks()
                             .GroupBy(chunk => chunk.Parent)
                             .Select(group => group.ToList()))
                {
                    if (chunkSiblings.Count == 0)
                    {
                        continue;
                    }

                    chunkSiblings.First().DeleteNodesInParent(chunkSiblings);
                }
            }
        }
        
        private bool CanGenerateMissingMaterials() => SelectedItem is ChunkViewModel
        {
            ResolvedData: CMesh,
            Parent: null
        };

        [RelayCommand(CanExecute = nameof(CanGenerateMissingMaterials))]
        private void OpenGenerateMaterialsDialog()
        {
            var dialog = new CreateMaterialsDialog();
            if (GetRoot() is not ChunkViewModel cvm || dialog.ShowDialog() != true)
            {
                return;
            }

            var baseMaterial = dialog.ViewModel?.BaseMaterial ?? "";
            var isLocal = dialog.ViewModel?.IsLocalMaterial ?? true;
            var resolveSubstitutions = dialog.ViewModel?.ResolveSubstitutions ?? false;

            cvm.GenerateMissingMaterials(baseMaterial, isLocal, resolveSubstitutions);

            cvm.Tab?.Parent.SetIsDirty(true);
        }

        #endregion

        /// <summary>
        /// Gets all selected chunks. If none are selected / if selection is invalid, it will return an empty list.
        /// </summary>
        private List<ChunkViewModel> GetSelectedChunks()
        {
            if (SelectedItems is not ObservableCollection<object> selection)
            {
                return [];
            }

            var uniqueItems = new HashSet<(object Parent, string Name)>();
            return selection.OfType<ChunkViewModel>().Where(x => uniqueItems.Add((x.Parent, x.Name))).ToList();
        }

        private void OnDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedItem is not ChunkViewModel chunk)
            {
                return;
            }

            // If the current chunk is not expanded, collapse all of its siblings
            if (!chunk.IsExpanded && chunk.Parent is ChunkViewModel parent)
            {
                chunk = parent;
            }

            var expansionState = chunk.IsExpanded;

            var expansionStates = chunk.GetAllProperties().Select((child) => child.IsExpanded).ToList();
            chunk.SetChildExpansionStates(!expansionStates.Contains(true));

            chunk.IsExpanded = expansionState;
        }

        private ChunkViewModel GetRoot()
        {
            if (ItemsSource is not ListCollectionView selection)
            {
                return null;
            }

            return selection.OfType<ChunkViewModel>().ToList().FirstOrDefault((cvm) => cvm.Parent is null);
        }

        private void RefreshSelectedItemsContextMenuFlags()
        {
            foreach (var chunkViewModel in GetSelectedChunks())
            {
                chunkViewModel.RefreshContextMenuFlags();
            }
        }

        private void TreeViewContextMenu_OnOpened(object sender, RoutedEventArgs e)
        {
            _isContextMenuOpen = true;
            RefreshSelectedItemsContextMenuFlags();
        }

        private void TreeViewContextMenu_OnClosed(object sender, RoutedEventArgs e) => _isContextMenuOpen = false;

        private void TreeViewContextMenu_OnKeyChanged(object sender, KeyEventArgs e)
        {
            _modifierViewStateSvc.OnKeystateChanged(e);
            RefreshSelectedItemsContextMenuFlags();
        }

        private bool _isContextMenuOpen;

        private void TreeView_OnKeyChanged(object sender, KeyEventArgs e)
        {
            if (_isContextMenuOpen)
            {
                return;
            }
            
            if (e.Key == Key.F2 && e.IsUp && CanOpenSearchAndReplaceDialog)
            {
                OpenSearchAndReplaceDialog();
                return;
            }

            _modifierViewStateSvc.OnKeystateChanged(e);
        }
        
        
    }
    
}
