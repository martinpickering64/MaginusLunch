using MaginusLunch.Core.Aggregates;
using MaginusLunch.Menu.Messages.Commands;
using MaginusLunch.Menu.Messages.Events;
using System;

namespace MaginusLunch.Menu.Domain
{
    public class MenuCategoryAggregate : AggregateRoot
    {
        public MenuCategoryAggregate(Guid id)
            : base()
        {
            Id = id;
        }

        #region Aggregate Properties

        public string EditorUserId { get; private set; }
        public string Name { get; private set; }

        #endregion Aggregate Properties

        #region Command Behaviours

        public MenuCategoryAggregate(CreateMenuCategory createMenuCategoryCommand)
            : this(createMenuCategoryCommand.Id)
        {
            RaiseEvent(new MenuCategoryAdded(createMenuCategoryCommand.Id, 
                                        createMenuCategoryCommand.Name, 
                                        createMenuCategoryCommand.EditorUserId));
        }

        public void ChangeName(ChangeNameOfMenuCategory command)
        {
            RaiseEvent(new MenuCategoryNameChanged(command.Id, command.NewName, command.EditorUserId));
        }

        #endregion Command Behaviours

        #region Event Handlers

        public void Apply(MenuCategoryAdded theEvent)
        {
            Id = theEvent.Id;
            EditorUserId = theEvent.EditorUserId;
            Name = theEvent.Name;
        }

        public void Apply(MenuCategoryNameChanged theEvent)
        {
            Name = theEvent.NewName;
        }

        #endregion Event Handlers

    }
}
