using System;

namespace MaginusLunch.Menu.Messages.Events
{
    public class FillingCreated : MenuEvent
    {
        public FillingCreated()
            : base()
        { }

        public FillingCreated(Guid fillingId, string name, MealType mealType, Guid lunchProviderId, string editorUserId)
            : base (fillingId, editorUserId)
        {
            Name = name;
            MealType = mealType;
            LunchProviderId = lunchProviderId;
            Status = FillingStatus.Available;
        }

        public string Name { get; set; }
        public MealType MealType { get; set; }
        public Guid LunchProviderId { get; set; }
        public FillingStatus Status { get; set; }
    }
}
