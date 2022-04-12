using System.Collections.Generic;

namespace Count4U.Model.Common
{
	public class Pair<TFirst, TSecond>
	{
		public Pair() { }

		public Pair(TFirst first, TSecond second)
			: this()
		{
			First = first;
			Second = second;
		}

		public TFirst First { get; set; }

		public TSecond Second { get; set; }

		public bool SmartEquals(object obj, bool orEquals = false)
		{
			if (orEquals)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				if (obj.GetType() != this.GetType()) return false;
				return EqualityComparer<TFirst>.Default.Equals(First, ((Pair<TFirst, TSecond>)obj).First) || EqualityComparer<TSecond>.Default.Equals(Second, ((Pair<TFirst, TSecond>)obj).Second);
			}
			else
			{
				return Equals(obj);
			}
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Pair<TFirst, TSecond>)obj);
		}

		public override string ToString()
		{
			return string.Format("First: {0},Second : {1}", this.First, this.Second);
		}

		protected bool Equals(Pair<TFirst, TSecond> other)
		{
			return EqualityComparer<TFirst>.Default.Equals(First, other.First) && EqualityComparer<TSecond>.Default.Equals(Second, other.Second);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (EqualityComparer<TFirst>.Default.GetHashCode(First) * 397) ^ EqualityComparer<TSecond>.Default.GetHashCode(Second);
			}
		}

		public bool IsEmpty()
		{
			return First == null && Second == null;
		}
	}

	public class Pair<TFirst, TSecond, TThird>
	{
		public Pair() { }

		public Pair(TFirst first, TSecond second, TThird third)
			: this()
		{
			First = first;
			Second = second;
			Third = third;
		}

		public TFirst First { get; set; }

		public TSecond Second { get; set; }

		public TThird Third { get; set; }

		public bool SmartEquals(object obj, bool orEquals = false)
		{
			if (orEquals)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				if (obj.GetType() != this.GetType()) return false;
				return EqualityComparer<TFirst>.Default.Equals(First, ((Pair<TFirst, TSecond, TThird>)obj).First)
					|| EqualityComparer<TSecond>.Default.Equals(Second, ((Pair<TFirst, TSecond, TThird>)obj).Second)
					|| EqualityComparer<TThird>.Default.Equals(Third, ((Pair<TFirst, TSecond, TThird>)obj).Third);
			}
			else
			{
				return Equals(obj);
			}
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Pair<TFirst, TSecond, TThird>)obj);
		}

		public override string ToString()
		{
			return string.Format("First: {0}, Second : {1}, Third : {2}", this.First, this.Second, this.Third);
		}

		protected bool Equals(Pair<TFirst, TSecond, TThird> other)
		{
			return EqualityComparer<TFirst>.Default.Equals(First, other.First)
				&& EqualityComparer<TSecond>.Default.Equals(Second, other.Second)
				&& EqualityComparer<TThird>.Default.Equals(Third, other.Third);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (EqualityComparer<TFirst>.Default.GetHashCode(First) * 397) ^ EqualityComparer<TSecond>.Default.GetHashCode(Second)
					^ EqualityComparer<TThird>.Default.GetHashCode(Third);
			}
		}

		public bool IsEmpty()
		{
			return First == null && Second == null && Third == null;
		}
	}
}
