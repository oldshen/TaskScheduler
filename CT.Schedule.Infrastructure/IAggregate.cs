namespace CT.Schedule.Domain
{
    public abstract class IAggregate {

    }
    /// <summary>
    /// 聚合根
    /// </summary>
    public abstract class IAggregate<T>: IAggregate
    {
        /// <summary>
        /// 标识ID
        /// </summary>
        public T Id { get; set; }
    }
}
