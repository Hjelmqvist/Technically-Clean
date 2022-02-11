using System;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using TMPro;

public class Settings : MonoBehaviour
{
    // Graphics
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    // UI elements
    public Slider masterSlider;
    public Slider sfxSlider;
    public Slider musicSlider;

    // PlayerPrefs variables
    const string MASTER_VOLUME_KEY = "master_volume";
    const string SFX_VOLUME_KEY = "sfx_volume";
    const string MUSIC_VOLUME_KEY = "music_volume";
    const string GRAPHICS_QUALITY_KEY = "graphics_quality";
    const string RESOLUTION_KEY = "resolution";

    // FMOD VCAs
    private VCA masterVCA;
    private VCA sfxVCA;
    private VCA musicVCA;

    List<Resolution> resolutions = new List<Resolution>();
    List<TMP_Dropdown.OptionData> dropdownOptions = new List<TMP_Dropdown.OptionData>();

    struct Resolution : IEquatable<Resolution>
    {
        public readonly int Width;
        public readonly int Height;

        public Resolution(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public bool Equals(Resolution other)
        {
            return other.Width == Width && other.Height == Height;
        }
    }

    public void SaveAndExit()
    {
        PlayerPrefs.Save();
    }

    public void ToggleFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    private void Awake()
    {
        masterVCA = RuntimeManager.GetVCA( "VCA:/_MasterVolume" );
        sfxVCA = RuntimeManager.GetVCA( "VCA:/SFX" );
        musicVCA = RuntimeManager.GetVCA( "VCA:/Music" );
    }

    private void Start()
    {
        InitializeAudio();
        InitializeGraphics();
    }

    private void Update()
    {
        SetVolume( masterVCA, masterSlider.value, MASTER_VOLUME_KEY );
        SetVolume( sfxVCA, sfxSlider.value, SFX_VOLUME_KEY );
        SetVolume( musicVCA, musicSlider.value, MUSIC_VOLUME_KEY );
    }

    private void InitializeGraphics()
    {
        SetupQualityDropdown();
        SetupResolutionDropdown();

        //If there is any old settings saved on the computer
        if (PlayerPrefs.HasKey( GRAPHICS_QUALITY_KEY ) && PlayerPrefs.HasKey( RESOLUTION_KEY ))
        {
            SetResolution( PlayerPrefs.GetInt( RESOLUTION_KEY ) );
            SetQualityLevel( PlayerPrefs.GetInt( GRAPHICS_QUALITY_KEY ) );
        }
        //Default values
        else
        {
            qualityDropdown.value = QualitySettings.GetQualityLevel();
            resolutionDropdown.value = Screen.resolutions.Length - 1;
            Screen.fullScreen = true;
            SetResolution( resolutions.Count - 1 );
            SetQualityLevel( qualityDropdown.options.Count - 1 );
        }
    }

    private void SetupQualityDropdown()
    {
        qualityDropdown.ClearOptions();
        List<TMP_Dropdown.OptionData> qualityOptions = new List<TMP_Dropdown.OptionData>();
        foreach (var quality in QualitySettings.names)
        {
            qualityOptions.Add( new TMP_Dropdown.OptionData( quality ) );
        }
        qualityDropdown.AddOptions( qualityOptions );
    }

    private void SetupResolutionDropdown()
    {
        resolutionDropdown.ClearOptions();
        foreach (var resolution in Screen.resolutions)
        {
            var res = new Resolution( resolution.width, resolution.height );
            if (resolutions.Contains( res ) == false)
            {
                resolutions.Add( res );
                dropdownOptions.Add( new TMP_Dropdown.OptionData( resolution.ToString().Split( '@' )[0] ) );
            }
        }
        resolutionDropdown.AddOptions( dropdownOptions );
    }

    public void SetQualityLevel(int i)
    {
        QualitySettings.SetQualityLevel( i, true );
        PlayerPrefs.SetInt( GRAPHICS_QUALITY_KEY, i );
        qualityDropdown.value = i;
        PlayerPrefs.Save();
    }

    public void SetResolution(int i)
    {
        Resolution res = resolutions[i];
        Screen.SetResolution( res.Width, res.Height, Screen.fullScreen );
        PlayerPrefs.SetInt( RESOLUTION_KEY, i );
        resolutionDropdown.value = i;
        PlayerPrefs.Save();
    }

    private void InitializeAudio()
    {
        if (PlayerPrefs.HasKey( MASTER_VOLUME_KEY ))
        {
            masterSlider.value = PlayerPrefs.GetFloat( MASTER_VOLUME_KEY );
        }
        else
        {
            masterSlider.value = 1f;
            sfxSlider.value = 1f;
            musicSlider.value = 1f;
            return;
        }

        if (PlayerPrefs.HasKey( SFX_VOLUME_KEY ))
        {
            sfxSlider.value = PlayerPrefs.GetFloat( SFX_VOLUME_KEY );
        }

        if (PlayerPrefs.HasKey( MUSIC_VOLUME_KEY ))
        {
            musicSlider.value = PlayerPrefs.GetFloat( MUSIC_VOLUME_KEY );
        }
    }

    private void SetVolume(VCA vca, float volume, string key)
    {
        vca.setVolume( volume );
        PlayerPrefs.SetFloat( key, volume );
        PlayerPrefs.Save();
    }
}