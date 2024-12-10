using System;

namespace Puzzle.Shared
{
    public class BaseEntity<T>
    {
        public T Id { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
