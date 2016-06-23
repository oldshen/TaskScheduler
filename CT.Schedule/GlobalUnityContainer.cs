using Microsoft.Practices.Unity;

namespace CT.Schedule
{
    /// <summary>
    /// 全局的Unity容器
    /// </summary>
    public static class GlobalUnityContainer
    {
        static readonly IUnityContainer _container;
        static GlobalUnityContainer()
        {
            _container = new UnityContainer();
        }

        /// <summary>
        /// 获得当前容器
        /// </summary>
        public static IUnityContainer Container
        {
            get
            {
                return _container;
            }
        }

        public static T Resolve<T>()
        {
            return Container.Resolve<T>();
        }
    }
}
