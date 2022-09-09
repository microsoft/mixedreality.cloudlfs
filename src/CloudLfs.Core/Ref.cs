namespace Microsoft.MixedReality.CloudLfs
{
    public class Ref<T> where T : struct
    {
        public T Value { get; set; }
        public Ref()
        {
            Value = default;
        }
        public Ref(T value) { Value = value; }
        public override string ToString() => Value.ToString();
        public static implicit operator T(Ref<T> r) { return r.Value; }
        public static implicit operator Ref<T>(T value) { return new Ref<T>(value); }
    }
}