using System.Reflection;

namespace Console_game
{
    public class Component
    {
        internal bool isInvoking = false;

        const BindingFlags defaultBindingFlags = BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.Instance |
            BindingFlags.IgnoreCase;

        public bool IsInvoking { get => isInvoking; }

        public void Invoke(MethodInfo method) => Invoke(method, null);

        public void Invoke(MethodInfo method, object[] parameters)
        {
            isInvoking = true;
            method.Invoke(this, parameters);
            isInvoking = false;
        }

        public MethodInfo GetMethod(string name) => GetMethod(name, defaultBindingFlags);

        public MethodInfo GetMethod(string name, BindingFlags bindingFlags)
        {
            return this.GetType().GetMethod(name, bindingFlags);
        }
    }
}
