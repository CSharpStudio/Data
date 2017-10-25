namespace Css.Data.SqlTree
{
    /*********************** 代码块解释 *********************************
     * 以下接口只为了作为开发过程中的强类型限制使用。
     * 文本可以使用在所有需要的类型上。
    **********************************************************************/

    public interface ISqlNode
    {
        SqlNodeType NodeType { get; }
    }
    public interface ISqlValue : ISqlNode { }
    public interface ISqlSelect : ISqlNode { }
    public interface ISqlConstraint : ISqlNode { }
    public interface ISqlSource : ISqlNode { }
    public interface IOrderBy : ISqlNode { }
    public interface IGroupBy : ISqlNode { }
    public interface ISqlLiteral : ISqlSelect, ISqlConstraint, ISqlSource { }
}
