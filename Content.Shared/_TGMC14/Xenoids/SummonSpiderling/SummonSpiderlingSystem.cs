using Content.Shared.Damage;
using Robust.Shared.Network;
using Robust.Shared.Audio.Systems;
using Content.Shared._TGMC14.Xenoids.Widow;
using Content.Shared._TGMC14.Xenoids.Spiderling;
using Content.Shared.Popups;

namespace Content.Shared._TGMC14.Xenoids.SummonSpiderling;
public sealed class SummonSpiderlingSystem : EntitySystem
{
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    
    public override void Initialize()
    {
        SubscribeLocalEvent<SummonSpiderlingComponent, SummonSpiderlingActionEvent>(OnSpiderlingSummon);
    }

    private void OnSpiderlingSummon(Entity<SummonSpiderlingComponent> xeno, ref SummonSpiderlingActionEvent args)
    {
        if (args.Handled)
            return;

        var wComp = EnsureComp<WidowComponent>(xeno);
        if (wComp.Spiders.Count >= wComp.MaxSpiders)
        {
            var msg = Loc.GetString("вы призвали максимально допустимое количиство слуг (" + wComp.MaxSpiders + ")");
            _popup.PopupEntity(msg, xeno);
            return;
        }

        if(_net.IsServer)
        {
            var spider = Spawn(xeno.Comp.Prototype, Transform(xeno).Coordinates);
            var sComp = EnsureComp<SpiderlingComponent>(spider);
            sComp.Widow = xeno;

            wComp.Spiders.Add(spider);

            _audio.PlayPvs(xeno.Comp.Sound, xeno);
        }

        _damageable.TryChangeDamage(xeno, xeno.Comp.Damage);

        args.Handled = true;
    }
}
