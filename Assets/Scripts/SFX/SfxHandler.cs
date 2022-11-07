using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class SfxHandler : MonoBehaviour
{
    [Header("Sound")]
    [SerializeField]
    protected AudioClip sfx;

    protected MMSoundManagerPlayOptions options = MMSoundManagerPlayOptions.Default;

    protected bool isInitialised = false;

    protected virtual void InitialiseSfx()
    {
        options.MmSoundManagerTrack = MMSoundManager.MMSoundManagerTracks.Sfx;
        options.Location = this.transform.position;

        isInitialised = true;
    }

    public virtual void PlaySfx()
    {
        if (!isInitialised)
        {
            InitialiseSfx();
        }

        MMSoundManagerSoundPlayEvent.Trigger(sfx, options);
    }
}
