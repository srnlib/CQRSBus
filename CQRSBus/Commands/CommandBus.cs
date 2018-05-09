﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using CQRSBus.DI;

namespace CQRSBus.Commands {
    public class CommandBus
    {
        private readonly IInjector injector;
        private readonly Dictionary<Type, Type> handlerTypes = new Dictionary<Type, Type>();
        private readonly Dictionary<Type, object> handlers = new Dictionary<Type, object>();

        public CommandBus(IInjector injector)
        {
            this.injector = injector;
        }

        public void Register<TCommand, THandler>()
            where TCommand : ICommand
            where THandler : ICommandHandler<TCommand>
        {
            handlerTypes.Add(typeof(TCommand), typeof(THandler));
        }

        public void RegisterInstance<TCommand, THandler>(THandler handler)
            where TCommand : ICommand
            where THandler : ICommandHandler<TCommand>
        {
            handlers.Add(typeof(TCommand), handler);
        }

        public void Handle<TCommand>(TCommand command)
            where TCommand : ICommand
        {
            var handler = GetHandler<TCommand>();
            handler.Handle(command);
        }

        private ICommandHandler<TCommand> GetHandler<TCommand>()
            where TCommand : ICommand
        {
            var commandType = typeof(TCommand);
            if (handlers.TryGetValue(commandType, out var searchedHandler))
            {
                return (ICommandHandler<TCommand>)searchedHandler;
            }
            
            var commandHandler = injector.Resolve<ICommandHandler<TCommand>>();
            handlers[commandType] = commandHandler;
            return commandHandler;
        }
    }
}
