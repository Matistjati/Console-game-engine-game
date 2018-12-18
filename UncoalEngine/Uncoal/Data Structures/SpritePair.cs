using Uncoal.Engine;

namespace Uncoal.Internal
{
	class SpritePair
	{
		public Sprite sprite;
		public string[,] newSprite;

		public SpritePair(Sprite oldSprite, string[,] NewSwprite)
		{
			sprite = oldSprite;
			newSprite = NewSwprite;
		}
	}
}
