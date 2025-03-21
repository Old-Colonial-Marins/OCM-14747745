﻿using System.Globalization;
using System.Numerics;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.XAML;

namespace Content.Client._CM14.Mortar;

[GenerateTypedNameReferences]
public sealed partial class MortarCoordsInputControl : Control
{
    public readonly int Index;
    public Vector2 Coords;

    public MortarCoordsInputControl(int index, Vector2 coords)
    {
        RobustXamlLoader.Load(this);

        Index = index;
        Coords = coords;

        CoordsX.Text = coords.X.ToString(CultureInfo.InvariantCulture);
        CoordsY.Text = coords.Y.ToString(CultureInfo.InvariantCulture);

        CoordsX.OnTextChanged += args =>
        {
            if (!float.TryParse(args.Text, out var value))
                return;

            Coords.X = value;
        };


        CoordsY.OnTextChanged += args =>
        {
            if (!float.TryParse(args.Text, out var value))
                return;

            Coords.Y = value;
        };
    }
}
