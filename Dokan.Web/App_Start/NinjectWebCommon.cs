[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Dokan.Web.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(Dokan.Web.App_Start.NinjectWebCommon), "Stop")]

namespace Dokan.Web.App_Start
{
    using System;
    using System.Data.Entity;
    using System.Web;
    using Dokan.Core.DataAccess;
    using Dokan.Core.Database;
    using Dokan.Services;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using Ninject.Web.Common.WebHost;

    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application.
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<DokanContext>().To<DokanContext>().InRequestScope();
            kernel.Bind(typeof(IRepository<>)).To(typeof(Repository<>)).InRequestScope();

            kernel.Bind<IUserService>().To<UserService>();
            kernel.Bind<IUserInformationService>().To<UserInformationService>();

            kernel.Bind<IMessageService>().To<MessageService>();

            kernel.Bind<IBlogPostService>().To<BlogPostService>();
            kernel.Bind<IBlogCategoryService>().To<BlogCategoryService>();
            kernel.Bind<IBlogCommentService>().To<BlogCommentService>();

            kernel.Bind<IProductService>().To<ProductService>();
            kernel.Bind<IProductCategoryService>().To<ProductCategoryService>();
            kernel.Bind<IProductCommentService>().To<ProductCommentService>();

            kernel.Bind<IDiscountCategoryService>().To<DiscountCategoryService>();
            kernel.Bind<ITaxCategoryService>().To<TaxCategoryService>();

            kernel.Bind<ICouponService>().To<CouponService>();

            kernel.Bind<IOrderService>().To<OrderService>();
            kernel.Bind<IOrderItemService>().To<OrderItemService>();

            kernel.Bind<IFileService>().To<FileService>();
            kernel.Bind<ILogService>().To<LogService>();

            kernel.Bind<IMenuService>().To<MenuService>();
            kernel.Bind<ICarouselService>().To<CarouselService>();
            kernel.Bind<ITestimonialService>().To<TestimonialService>();
            kernel.Bind<IKeyValueContentService>().To<KeyValueContentService>();
        }
    }
}