using Microsoft.Xna.Framework;

namespace Bloodbender.Enemies.Scenario3
{
    public class HorizontalInsideWall : PhysicObj
    {
        public HorizontalInsideWall(Vector2 position) : base(position, PathFinderNodeType.OUTLINE)
        {
            var fix = createRectangleFixture(200, 70, Vector2.Zero, new AdditionalFixtureData(this, HitboxType.BOUND));
            //var fix = createOctogoneFixture(300, 300, Vector2.Zero);
            createOutlinePathNodes();
            body.IsStatic = true;
        } 
    }
}
