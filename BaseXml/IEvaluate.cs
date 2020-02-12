namespace BaseXml
{
    public interface IEvaluate
    {
        T Evaluate<T>(XPath xPath, T @default = default);
    }
}
