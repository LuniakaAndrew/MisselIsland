using System;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public enum CellStates
    {
        ground,
        water,
        destroy,
        building,
        prepare,
        shield,
        fog,
        boom,
        error
    };
    public enum GameStates
    {
        Start,
        End,
        EnemyAttack,
        EnemyBuild,
        PlayerAttack,
        PalyerBuild
    };
}
