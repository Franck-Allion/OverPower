namespace OverPower.Domain.Primitives
{
    public readonly struct DomainError
    {
        public string Code { get; }
        public string Message { get; }

        public DomainError(string code, string message)
        {
            Code = code ?? throw new System.ArgumentNullException(nameof(code));
            Message = message ?? throw new System.ArgumentNullException(nameof(message));
        }

        public override string ToString() => $"[{Code}] {Message}";
    }
}
