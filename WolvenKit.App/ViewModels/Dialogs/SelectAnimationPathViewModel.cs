using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using WolvenKit.Interfaces.Extensions;
using WolvenKit.RED4.Types;


namespace WolvenKit.App.ViewModels.Dialogs;

/// <summary>
/// Asks user for a component name and a chunk mask
/// </summary>
public partial class SelectAnimationPathViewModel : ObservableObject
{
    [GeneratedRegex(@"(?<=__)([^\\]+)(?=_rigsetup)")]
    private static partial Regex AnimNameRegex();

    [ObservableProperty] private string? _rigPath = "";
    [ObservableProperty] private string? _animationPath = "";
    [ObservableProperty] private string? _selectedAnimEntrySet = "";
    [ObservableProperty] private List<string>? _selectedAnimEntries;
    [ObservableProperty] private bool _isRenameAnimsForPhotomode;

    public SelectAnimationPathViewModel(List<string> facialSetupPaths) =>
        facialSetupPaths.Where(path => !_animGraphOptions.ContainsValue(path)).ToList().ForEach(path =>
        {
            if (AnimNameRegex().Matches(path).FirstOrDefault()?.Value is not string animName)
            {
                return;
            }

            var bodyGender = path.Contains("_wa_") || path.Contains("_pwa_") ? "Woman" : "Man";
            animName = animName.Replace("_", " ").CapitalizeEachWord();
            var animKey = $"{bodyGender} - {animName}";
            var numDuplicates = _animGraphOptions.Keys.Count((item) => item.StartsWith(animKey));
            if (numDuplicates > 0)
            {
                animKey = $"{animKey} {numDuplicates}";
            }

            _animGraphOptions.Add(animKey, path);
            FilteredGraphOptions = _animGraphOptions;
        });

    private Dictionary<string, string> _facialAnimOptions = new()
    {
        // Photo mode
        { "Photo Mode: Player Woman", @"base\animations\facial\_facial_graphs\player_woman_photomode_sermo.animgraph" },
        { "Photo Mode: Player Man", @"base\animations\facial\_facial_graphs\player_man_photomode_sermo.animgraph" },

        // Spawned NPC
        { "NPV: Woman Player", @"base\animations\facial\_facial_graphs\player_woman_paperdoll_sermo.animgraph" },
        { "NPV: Woman Average", @"base\animations\facial\_facial_graphs\woman_average_sermo.animgraph" },
        { "NPV: Woman Average (LC)", @"base\animations\facial\_facial_graphs\woman_average_sermo_lc.animgraph" },
        { "NPV: Man Player", @"base\animations\facial\_facial_graphs\player_man_paperdoll_sermo.animgraph" },
        { "NPV: Man Big", @"base\animations\facial\_facial_graphs\man_big_sermo.animgraph" },
        { "NPV: Man Average", @"base\animations\facial\_facial_graphs\man_average_sermo.animgraph" },
        { "NPV: Fat", @"base\animations\facial\_facial_graphs\man_fat_sermo.animgraph" },
    };

    private readonly Dictionary<string, string> _animGraphOptions = new()
    {
        // more entries will be appended by list of paths from constructor
        { "Player Woman", @"base\characters\head\pma\h0_001_ma_c__player\h0_001_ma_c__player_rigsetup.facialsetup" },
        { "Player Man", @"base\characters\head\pma\h0_001_ma_c__player\h0_001_ma_c__player_rigsetup.facialsetup" },
    };

    private const string s_animEntryNpc = "NPC";
    private const string s_animEntryPhotomodeM = "Photomode (male)";
    private const string s_animEntryPhotomodeF = "Photomode (female)";

    private static readonly List<string> PhotomodeAnimEntriesFemale =
    [
        "base\\animations\\ui\\photomode\\photomode_female_facial.anims",
        "base\\animations\\xbaebsae\\pm_facials\\fem\\xbae_pm_facials_01.anims",
        "base\\animations\\xbaebsae\\pm_facials\\fem\\xbae_pm_facials_02.anims",
        "base\\animations\\xbaebsae\\pm_facials\\fem\\xbae_pm_facials_03.anims",
        "base\\animations\\xbaebsae\\pm_facials\\fem\\xbae_pm_facials_04.anims",
        "base\\animations\\xbaebsae\\pm_facials\\fem\\xbae_pm_facials_05.anims",
        "base\\animations\\xbaebsae\\pm_facials\\fem\\xbae_pm_facials_06.anims",
        "base\\animations\\xbaebsae\\pm_facials\\fem\\xbae_pm_facials_07.anims",
        "base\\animations\\xbaebsae\\pm_facials\\fem\\xbae_pm_facials_08.anims",
        "base\\animations\\xbaebsae\\pm_facials\\fem\\xbae_pm_facials_09.anims",
        "base\\animations\\xbaebsae\\pm_facials\\fem\\xbae_pm_facials_10.anims",
        "base\\animations\\xbaebsae\\pm_facials\\fem\\xbae_pm_facials_11.anims",
        "base\\animations\\xbaebsae\\pm_facials\\fem\\xbae_pm_facials_12.anims",
        "base\\animations\\xbaebsae\\pm_facials\\fem\\xbae_pm_facials_13.anims",
        "base\\animations\\xbaebsae\\pm_facials\\fem\\xbae_pm_facials_14.anims",
        "base\\animations\\xbaebsae\\pm_facials\\fem\\xbae_pm_facials_15.anims",
    ];

    private static readonly List<string> PhotomodeAnimEntriesMale = PhotomodeAnimEntriesFemale
        .Select(filePath => filePath.Replace("_female_", "_male")).ToList();

    private readonly List<string> NPCAnimEntries =
    [
        "base\\animations\\facial\\male_average\\interactive_scene\\e3_male_average_custom_animations.anims",
        "base\\animations\\facial\\male_average\\interactive_scene\\generic_average_male_facial_idle.anims",
        "base\\animations\\facial\\male_average\\interactive_scene\\generic_average_male_facial_transitions.anims",
        "base\\animations\\facial\\male_average\\interactive_scene\\generic_average_male_facial_transitions_correctives.anims",
        "base\\animations\\facial\\male_average\\interactive_scene\\generic_average_male_facial_gestures.anims",
        "base\\animations\\facial\\male_average\\interactive_scene\\generic_average_male_facial_quirks.anims",
        "base\\animations\\facial\\male_average\\interactive_scene\\generic_average_male_facial_custom_animations.anims",
        "base\\animations\\cyberware\\mantisblade\\mantisblade_01_enemy_face.anims",
        "base\\animations\\facial\\quest_custom\\jackie\\jackie_facial.anims",
        "base\\animations\\facial\\female_average\\interactive_scene\\generic_average_female_facial_idle.anims",
        "base\\animations\\facial\\female_average\\interactive_scene\\generic_average_female_facial_transitions.anims",
        "base\\animations\\facial\\female_average\\interactive_scene\\generic_average_female_facial_transitions_correctives.anims",
        "base\\animations\\facial\\female_average\\interactive_scene\\generic_average_female_facial_gestures.anims",
        "base\\animations\\facial\\female_average\\interactive_scene\\generic_average_female_facial_custom_animations.anims",
        "base\\animations\\facial\\female_average\\interactive_scene\\generic_average_female_facial_quirks.anims",
        "base\\animations\\facial\\generic\\interactive_scene\\generic_facial_additives.anims",
        "base\\animations\\facial\\generic\\interactive_scene\\generic_facial_combat.anims",
        "base\\animations\\facial\\male_average\\interactive_scene\\generic_average_male_facial_idle_poses.anims",
        "base\\animations\\facial\\female_average\\interactive_scene\\generic_average_female_facial_idle_poses.anims",
        "base\\animations\\quest\\main_quests\\prologue\\q003\\q003_01a_militech\\anim\\facial\\q003_01a_militech__female_average__facial.anims",
        "base\\animations\\quest\\main_quests\\prologue\\q003\\q003_01a_militech\\anim\\facial\\q003_01a_militech__male_average__facial.anims",
        "base\\animations\\quest\\main_quests\\prologue\\q003\\q003_01a_militech\\anim\\facial\\q003_01a_militech__male_big__facial.anims",
        "base\\animations\\quest\\main_quests\\prologue\\q003\\q003_07aa_final_traitor\\anim\\facial\\q003_07aa_final_traitor__male_average__facial.anims",
        "base\\animations\\facial\\generic\\interactive_scene\\generic_facial_lipsync_gestures.anims",
        "base\\animations\\cyberware\\personal_link\\personal_link_takedown_face.anims",
        "base\\animations\\facial\\gameplay\\mb_environmental_takedowns\\face_environmental_takedowns.anims",
        "base\\animations\\npc\\gameplay\\man_massive\\gang\\unarmed\\synced\\face_mm_ground_and_pound_sync.anims",
        "base\\animations\\npc\\gameplay\\man_big\\gang\\unarmed\\finishers\\face_mb_fist_finisher.anims",
        "base\\animations\\npc\\gameplay\\man_average\\gang\\unarmed\\takedowns\\face_ma_grapple_sync.anims",
        "base\\animations\\facial\\gameplay\\face_reaction_base.anims",
        "base\\animations\\cyberware\\mantisblade\\face_ma_mantisblade_finisher.anims",
        "base\\animations\\facial\\generic\\interactive_scene\\generic_facial_gestures.anims",
        "base\\animations\\npc\\gameplay\\man_average\\corpo\\katana\\facial\\facial_corpo_katana_action_combat.anims",
        "base\\animations\\npc\\gameplay\\man_average\\gang\\unarmed\\takedowns\\face_ma_takedown_sync.anims",
        "base\\animations\\facial\\main_characters\\evelyn\\evelyn_facial_custom_animations.anims",
        "base\\animations\\facial\\quest_custom\\sandra\\sandra__facial_custom_animations.anims",
        "base\\animations\\facial\\main_characters\\river\\river_facial_custom_animation.anims",
        "base\\animations\\quest\\main_quests\\part1\\q108\\q108_03_alt\\synced\\synced__q108_03_alt__alt_face.anims",
        "base\\animations\\facial\\quest_custom\\alt\\alt__facial_custom_animations.anims",
        "base\\animations\\facial\\main_characters\\panam\\panam_facial_custom_animations.anims",
        "base\\animations\\items\\interactive\\garbage\\face_mb_chute.anims",
        "base\\animations\\facial\\quest_custom\\joshua_stephenson\\joshua_facial_custom_animations.anims",
        "base\\animations\\quest\\main_quests\\part1\\q101\\03_car_ride\\q101_03_car_ride__sequence_3\\synced\\synced_q101_03_car_ride_seq_3__troy_1_face_animation.anims",
        "base\\animations\\facial\\quest_custom\\troy\\synced_q101_03_car_ride_seq_3__troy_1_face_animation.anims",
        "base\\animations\\quest_custom\\mws_hey_01\\locomotion\\mws_hey_01_child_locomotion_facial.anims",
        "base\\animations\\weapon\\katana\\face_ma_katana_finisher.anims",
        "base\\animations\\weapon\\knife\\face_ma_knife_finisher.anims",
        "base\\animations\\weapon\\melee_fists\\face_ma_fist_finisher.anims",
        "base\\animations\\weapon\\two_handed_blunt\\face_ma_two_handed_blunt_finisher.anims",
        "base\\animations\\weapon\\two_handed_hammer\\face_ma_two_handed_hammer_finisher.anims",
        "base\\animations\\weapon\\one_handed_blunt\\face_ma_finisher_one_handed_blunt.anims",
        "base\\animations\\weapon\\one_handed_blunt\\face_one_handed_blunt_dildo_finisher.anims",
        "base\\animations\\cyberware\\strongarms\\face_ma_strongarms_finisher.anims",
        "base\\animations\\cyberware\\monowire\\face_ma_monowire_finisher.anims",
        "base\\animations\\weapon\\baton\\face_baton_finisher.anims",
        "base\\animations\\weapon\\one_handed_blunt\\face_axe_finisher.anims",
        "base\\animations\\facial\\gameplay\\face_ma_gang_unarmed_reaction_death.anims",
    ];

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(FilterText):
                FilteredGraphOptions = string.IsNullOrEmpty(FilterText)
                    ? _animGraphOptions
                    : _animGraphOptions.Where(o =>
                        o.Key.Contains(FilterText, StringComparison.OrdinalIgnoreCase) ||
                        o.Value.Contains(FilterText, StringComparison.OrdinalIgnoreCase));
                break;
            case nameof(SelectedAnimEntrySet):
                switch (SelectedAnimEntrySet)
                {
                    case s_animEntryNpc:
                        SelectedAnimEntries = NPCAnimEntries;
                        break;
                    case s_animEntryPhotomodeF:
                        SelectedAnimEntries = PhotomodeAnimEntriesFemale;
                        break;
                    case s_animEntryPhotomodeM:
                        SelectedAnimEntries = PhotomodeAnimEntriesMale;
                        break;
                    default:
                        break;
                }

                break;
            default:
                break;
        }
    }



    [ObservableProperty] private string? _selectedFacialAnim;
    public string? SelectedAnimPath => SelectedFacialAnim is null ? null : _facialAnimOptions[SelectedFacialAnim];

    // ReSharper disable once UnusedMember.Global used in xaml
    public Dictionary<string, string> FacialAnimOptions
    {
        get => _facialAnimOptions;
        set => SetProperty(ref _facialAnimOptions, value);
    }

    // ReSharper disable once UnusedMember.Global used in xaml
    public Dictionary<string, string> AnimEntryOptions { get; } = new()
    {
        { s_animEntryNpc, s_animEntryNpc },
        { s_animEntryPhotomodeF, s_animEntryPhotomodeF },
        { s_animEntryPhotomodeM, s_animEntryPhotomodeM },
    };

    [ObservableProperty] private string? _selectedGraph;

    public string? SelectedGraphPath => SelectedGraph is null ? null : _animGraphOptions[SelectedGraph];

    [ObservableProperty] private string? _filterText;

    [ObservableProperty] private IEnumerable<KeyValuePair<string, string>> _filteredGraphOptions = [];
}