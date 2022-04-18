namespace Devit.Commanding
{
    public enum UpdateOperation
    {
        NotProvided = 0,
        Set,
    }

    public class Update
    {
        public static readonly Update NotProvided = new Update(UpdateOperation.NotProvided);

        private readonly UpdateOperation _operation;

        public UpdateOperation Operation => _operation;

        public Update(UpdateOperation operation)
        {
            _operation = operation;
        }
    }

    public class Update<T> where T : notnull
    {
        private readonly UpdateOperation _operation;
        private readonly T? _value;

        public UpdateOperation Operation => _operation;

        public Update(T? value, UpdateOperation operation)
        {
            _value = value;
            _operation = operation;
        }

        public T GetValue()
        {
            if (_operation == UpdateOperation.NotProvided)
            {
                throw new InvalidOperationException();
            }

            return _value!;
        }

        public static implicit operator Update<T>(Update update)
        {
            return new Update<T>(default, update.Operation);
        }

        public static implicit operator Update<T>(T value)
        {
            return new Update<T>(value, UpdateOperation.Set);
        }

        public static explicit operator T(Update<T> thing)
        {
            return thing.GetValue();
        }
    }
}
