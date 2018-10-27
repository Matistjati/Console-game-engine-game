using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_game
{
	class SampleGameObj : GameObject
	{
		int SampleGameObjj;
		public SampleGameObj()
		{
			SpriteDisplayer sprite = AddComponent<SpriteDisplayer>();
			//physicalState.Scale = 1f;
			sprite.SetImage("sample3.png");
			sprite.Layer = 1;

			SampleComponent sampleTest = AddComponent<SampleComponent>();
		}
	}
}
