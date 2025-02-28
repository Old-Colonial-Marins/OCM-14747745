using Robust.Shared.Audio.Systems;
using Robust.Shared.Random;
using Robust.Shared.Player;
using Content.Server.Audio;
using Robust.Server.Player;

namespace Content.Server._CM14.BarrMusic;

public sealed class BarrMusicSystem : EntitySystem
{
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly ServerGlobalSoundSystem _sound = default!;

    private Random _rand = new Random();

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<BarrMusicComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            if (!comp.Enabled)
                continue;

            comp.PlayMusicNext += TimeSpan.FromSeconds(frameTime);

            if (comp.PlayMusicTime > comp.PlayMusicNext)
                continue;

            var music = _rand.Pick(comp.MusicPool);
            comp.PlayMusicNext = TimeSpan.Zero;
            _sound.PlayAdminGlobal(Filter.Empty().AddAllPlayers(_playerManager), _audio.GetSound(music), music.Params, false);
        }
    }

    public void SetEnabledForAllMusicComps(bool enabled)
    {
        var query = EntityQueryEnumerator<BarrMusicComponent>();
        while (query.MoveNext(out _, out var music))
        {
            music.PlayMusicNext = TimeSpan.Zero;
            music.Enabled = enabled;
        }
    }
}
