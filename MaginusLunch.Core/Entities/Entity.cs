namespace MaginusLunch.Core.Entities
{
    using System;

    /// <summary>
    /// Abstract Entity
    /// </summary>
    /// <remarks>
    /// Using Object.ReferenceEquals in this implementation when checking for NULL.
    /// This prevents a recursive loop due to the overriding of equality operators.
    /// </remarks>
    public abstract class Entity : IEntity, 
                                    IComparable, IComparable<Entity>, 
                                    IEquatable<Entity>, 
                                    ICloneable
    {
        protected Entity(Guid id)
        {
            Id = id;
        }

        /// <summary>
        /// The Object Id of the Entity
        /// </summary>
        public Guid Id { get; private set; }

        public override string ToString()
        {
            return $"{GetType().Name}:{Id.ToString()}";
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public abstract object Clone();

        public virtual int CompareTo(object other)
        {
            if (ReferenceEquals(null, other)) return 1;
            if (!(other is Entity))
            {
                throw new ArgumentException($"You must compare an Entity with another Entity. Attempt was to compare a {this.GetType().Name} with a {other.GetType().Name}.", "other");
            }
            return CompareTo((Entity)other);
        }

        public virtual int CompareTo(Entity other)
        {
            if (ReferenceEquals(null, other)) return 1;
            return Id.CompareTo(other.Id);
        }

        public virtual bool Equals(Entity other)
        {
            if (ReferenceEquals(null, other)) return false;
            return Id == other.Id;
        }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (!(other is Entity))
            {
                return false;
            }
            return Equals((Entity)other);
        }

        public static bool Equals(Entity a, Entity b)
        {
            return a.Equals(b);
        }

        public static bool operator == (Entity a, Entity b)
        {
            if (ReferenceEquals(null, a) && ReferenceEquals(null, b)) return true;
            if (ReferenceEquals(null, a)) return false;
            return a.Equals(b);
        }

        public static bool operator != (Entity a, Entity b)
        {
            return !(a == b);
        }

        public static bool operator <(Entity left, Entity right)
        {
            return ReferenceEquals(left, null) ? !ReferenceEquals(right, null) : left.CompareTo(right) < 0;
        }

        public static bool operator <=(Entity left, Entity right)
        {
            return ReferenceEquals(left, null) || left.CompareTo(right) <= 0;
        }

        public static bool operator >(Entity left, Entity right)
        {
            return !ReferenceEquals(left, null) && left.CompareTo(right) > 0;
        }

        public static bool operator >=(Entity left, Entity right)
        {
            return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
        }
    }

    /// <summary>
    /// Abstract Entity wrapper for non-edittable models
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <example>
    /// public class Person
    /// {
    ///     public int PersonId {get; set;}
    ///     public string Name {get; set;}
    /// }
    /// 
    /// public class PersonEntity : Entity<Person>
    /// {
    ///     public PersonEntity(Guid id):base(id) {}
    /// }
    /// </example>
    public abstract class Entity<T> : Entity
    {
        protected Entity(Guid id) : base(id) { }

        /// <summary>
        /// Placeholder for the wrapped object
        /// </summary>
        public T Content { get; set; }
    }
}
