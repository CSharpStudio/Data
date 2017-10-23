﻿using Castle.MicroKernel.Registration;
using Castle.Windsor;
using System;
using System.Linq;
using System.Reflection;

namespace Css.Services
{
    public class ServiceContainer : IServiceContainer
    {
        /// <summary>
        /// Reference to the Castle Windsor Container.
        /// </summary>
        public IWindsorContainer WindsorContainer { get; set; }

        /// <summary>
        /// Creates a new <see cref="ServiceContainer"/> object.
        /// Normally, you don't directly instantiate an <see cref="ServiceContainer"/>.
        /// This may be useful for test purposes.
        /// </summary>
        public ServiceContainer()
        {
            WindsorContainer = new WindsorContainer();

            //Register self!
            WindsorContainer.Register(Component.For<ServiceContainer, IServiceContainer, IServiceRegistrar, IServiceResolver>().UsingFactoryMethod(() => this))
                .Register(Component.For<RemoteServiceInterceptor>());

        }

        /// <summary>
        /// Registers a type as self registration.
        /// </summary>
        /// <typeparam name="TType">Type of the class</typeparam>
        /// <param name="lifeStyle">Lifestyle of the objects of this type</param>
        public void Register<TType>(ServiceLifeStyle lifeStyle = ServiceLifeStyle.Singleton) where TType : class
        {
            WindsorContainer.Register(ApplyLifestyle(Component.For<TType>(), lifeStyle));
        }

        /// <summary>
        /// Registers a type as self registration.
        /// </summary>
        /// <param name="type">Type of the class</param>
        /// <param name="lifeStyle">Lifestyle of the objects of this type</param>
        public void Register(Type type, ServiceLifeStyle lifeStyle = ServiceLifeStyle.Singleton)
        {
            WindsorContainer.Register(ApplyLifestyle(Component.For(type), lifeStyle));
        }

        /// <summary>
        /// Registers a type with it's implementation.
        /// </summary>
        /// <typeparam name="TType">Registering type</typeparam>
        /// <typeparam name="TImpl">The type that implements TType/></typeparam>
        /// <param name="lifeStyle">Lifestyle of the objects of this type</param>
        public void Register<TType, TImpl>(ServiceLifeStyle lifeStyle = ServiceLifeStyle.Singleton)
            where TType : class
            where TImpl : class, TType
        {
            WindsorContainer.Register(ApplyLifestyle(Component.For<TType, TImpl>().ImplementedBy<TImpl>(), lifeStyle));
        }

        /// <summary>
        /// Registers a type with it's implementation.
        /// </summary>
        /// <param name="type">Type of the class</param>
        /// <param name="impl">The type that implements <paramref name="type"/></param>
        /// <param name="lifeStyle">Lifestyle of the objects of this type</param>
        public void Register(Type type, Type impl, ServiceLifeStyle lifeStyle = ServiceLifeStyle.Singleton)
        {
            WindsorContainer.Register(ApplyLifestyle(Component.For(type, impl).ImplementedBy(impl), lifeStyle));
        }

        public void Register(Type type, object instance)
        {
            WindsorContainer.Register(Component.For(type).Instance(instance));
        }

        public void Register<T>(T instance) where T : class
        {
            WindsorContainer.Register(Component.For<T>().Instance(instance));
        }

        /// <summary>
        /// Checks whether given type is registered before.
        /// </summary>
        /// <param name="type">Type to check</param>
        public bool IsRegistered(Type type)
        {
            return WindsorContainer.Kernel.HasComponent(type);
        }

        /// <summary>
        /// Checks whether given type is registered before.
        /// </summary>
        /// <typeparam name="TType">Type to check</typeparam>
        public bool IsRegistered<TType>()
        {
            return WindsorContainer.Kernel.HasComponent(typeof(TType));
        }

        /// <summary>
        /// Gets an object from IOC container.
        /// Returning object must be Released (see <see cref="IServiceResolver.Release"/>) after usage.
        /// </summary> 
        /// <typeparam name="T">Type of the object to get</typeparam>
        /// <returns>The instance object</returns>
        public T Resolve<T>()
        {
            TryRegisterFallback(typeof(T));
            return WindsorContainer.Resolve<T>();
        }

        void TryRegisterFallback(Type type)
        {
            if (!IsRegistered(type))
            {
                lock (WindsorContainer)
                {
                    if (!IsRegistered(type))
                    {
                        var attr = type.GetCustomAttribute<ServiceAttribute>(false);
                        if (attr != null && attr.FallbackType != null)
                            Register(type, attr.FallbackType);
                    }
                }
            }
        }

        /// <summary>
        /// Gets an object from IOC container.
        /// Returning object must be Released (see <see cref="Release"/>) after usage.
        /// </summary> 
        /// <typeparam name="T">Type of the object to cast</typeparam>
        /// <param name="type">Type of the object to resolve</param>
        /// <returns>The object instance</returns>
        public T Resolve<T>(Type type)
        {
            TryRegisterFallback(type);
            return (T)WindsorContainer.Resolve(type);
        }

        /// <summary>
        /// Gets an object from IOC container.
        /// Returning object must be Released (see <see cref="IServiceResolver.Release"/>) after usage.
        /// </summary> 
        /// <typeparam name="T">Type of the object to get</typeparam>
        /// <param name="argumentsAsAnonymousType">Constructor arguments</param>
        /// <returns>The instance object</returns>
        public T Resolve<T>(object argumentsAsAnonymousType)
        {
            TryRegisterFallback(typeof(T));
            return WindsorContainer.Resolve<T>(argumentsAsAnonymousType);
        }

        /// <summary>
        /// Gets an object from IOC container.
        /// Returning object must be Released (see <see cref="IServiceResolver.Release"/>) after usage.
        /// </summary> 
        /// <param name="type">Type of the object to get</param>
        /// <returns>The instance object</returns>
        public object Resolve(Type type)
        {
            TryRegisterFallback(type);
            return WindsorContainer.Resolve(type);
        }

        /// <summary>
        /// Gets an object from IOC container.
        /// Returning object must be Released (see <see cref="IServiceResolver.Release"/>) after usage.
        /// </summary> 
        /// <param name="type">Type of the object to get</param>
        /// <param name="argumentsAsAnonymousType">Constructor arguments</param>
        /// <returns>The instance object</returns>
        public object Resolve(Type type, object argumentsAsAnonymousType)
        {
            TryRegisterFallback(type);
            return WindsorContainer.Resolve(type, argumentsAsAnonymousType);
        }

        ///<inheritdoc/>
        public T[] ResolveAll<T>()
        {
            return WindsorContainer.ResolveAll<T>();
        }

        ///<inheritdoc/>
        public T[] ResolveAll<T>(object argumentsAsAnonymousType)
        {
            return WindsorContainer.ResolveAll<T>(argumentsAsAnonymousType);
        }

        ///<inheritdoc/>
        public object[] ResolveAll(Type type)
        {
            return WindsorContainer.ResolveAll(type).Cast<object>().ToArray();
        }

        ///<inheritdoc/>
        public object[] ResolveAll(Type type, object argumentsAsAnonymousType)
        {
            return WindsorContainer.ResolveAll(type, argumentsAsAnonymousType).Cast<object>().ToArray();
        }

        /// <summary>
        /// Releases a pre-resolved object. See Resolve methods.
        /// </summary>
        /// <param name="obj">Object to be released</param>
        public void Release(object obj)
        {
            WindsorContainer.Release(obj);
        }

        static ComponentRegistration<T> ApplyLifestyle<T>(ComponentRegistration<T> registration, ServiceLifeStyle lifeStyle)
            where T : class
        {
            //if (typeof(T).IsAssignableFrom(typeof(RemoteService)))
            //    registration.Interceptors<RemoteServiceInterceptor>();
            switch (lifeStyle)
            {
                case ServiceLifeStyle.Transient:
                    return registration.LifestyleTransient();
                case ServiceLifeStyle.Singleton:
                    return registration.LifestyleSingleton();
                default:
                    return registration;
            }
        }
    }
}