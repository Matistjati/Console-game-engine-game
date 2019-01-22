using Uncoal.Engine;
using static Uncoal.Internal.NativeMethods;

namespace Uncoal.Internal
{
	class SpritePair
	{
		public Sprite sprite;
		public CHAR_INFO[,] newSprite;

		public SpritePair(Sprite oldSprite, CHAR_INFO[,] NewSwprite)
		{
			sprite = oldSprite;
			newSprite = NewSwprite;
		}
	}
}
