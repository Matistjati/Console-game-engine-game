using System;

namespace Console_game
{
    class ComponentTest : Component
    {
        public int i = 10;
        void update()
        {
            Console.Write(i);
        }

        void start()
        {
            Console.Write("hoi");
        }

        bool methodFound()
        {
            return true;
        }
    }
}
