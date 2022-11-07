using MoreMountains.Tools;
using MoreMountains.TopDownEngine;

public class LoopingSfxHandler : SfxHandler, MMEventListener<TopDownEngineEvent>
{
    private int id = 1;
    private bool isPlaying;

    private void OnEnable()
    {
        this.MMEventStartListening();
        this.isPlaying = false;
    }

    private void OnDisable()
    {
        this.MMEventStopListening();
    }

    protected override void InitialiseSfx()
    {
        this.id = IdAssigner.GetSoundId();
        options.ID = this.id;
        options.Loop = true;

        base.InitialiseSfx();
    }

    public override void PlaySfx()
    {
        if (!isInitialised)
        {
            InitialiseSfx();
        }

        StopSfx();
        MMSoundManagerSoundPlayEvent.Trigger(sfx, options);
        this.isPlaying = true;
    }

    public void StopSfx()
    {
        MMSoundManagerSoundControlEvent.Trigger(MMSoundManagerSoundControlEventTypes.Free, id);
        this.isPlaying = false;
    }

    public void OnMMEvent(TopDownEngineEvent eventType)
    {
        if (!isPlaying)
        {
            return;
        }

        if (eventType.EventType == TopDownEngineEventTypes.Pause)
        {
            MMSoundManagerSoundControlEvent.Trigger(MMSoundManagerSoundControlEventTypes.Pause, id);
        } 
        else if (eventType.EventType == TopDownEngineEventTypes.UnPause)
        {
            MMSoundManagerSoundControlEvent.Trigger(MMSoundManagerSoundControlEventTypes.Resume, id);
        }
    }
}
