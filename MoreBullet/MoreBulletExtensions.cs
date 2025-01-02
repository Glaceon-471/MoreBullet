using Harmony;

namespace MoreBullet
{
    public static class MoreBulletExtensions
    {
        public static void RunMethod(this object obj, string name, params object[] parameters)
        {
            obj.GetType().GetMethod(name, AccessTools.all).Invoke(obj, parameters);
        }

        public static T RunMethod<T>(this object obj, string name, params object[] parameters)
        {
            return (T)obj.GetType().GetMethod(name, AccessTools.all).Invoke(obj, parameters);
        }
    }
}
