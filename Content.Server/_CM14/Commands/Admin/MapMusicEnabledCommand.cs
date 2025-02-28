using Content.Server._CM14.BarrMusic;
using Content.Server._CM14.Requisitions;
using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server._CM14.Commands.Admin;

[AdminCommand(AdminFlags.Admin)]
public sealed class BarrMusicEnabledCommand : IConsoleCommand
{
    [Dependency] private readonly IEntityManager _entity = default!;

    public string Command => "barr_music_enabled";
    public string Description => "Turn on or off in-game music that's plays every 25 minutes";
    public string Help => "barr_music_enabled <bool>";

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length != 1)
        {
            shell.WriteError(Loc.GetString("shell-argument-count-must-be-between", ("lower", 1), ("upper", 1)));
            return;
        }

        if (!bool.TryParse(args[0], out var enabled))
        {
            shell.WriteError(Loc.GetString("shell-invalid-int"));
            return;
        }

        var musicSys = _entity.System<BarrMusicSystem>();
        musicSys.SetEnabledForAllMusicComps(enabled);

        string isEnabled = enabled ? "enabled" : "disabled";
        shell.WriteLine($"Music is {isEnabled}.");
    }
}
