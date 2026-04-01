
/// <summary>
/// 操作结果
/// </summary>
public struct OperateResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccess;
    /// <summary>
    /// 描述
    /// </summary>
    public string Description;
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="isSuccess">是否成功</param>
    /// <param name="description">描述</param>
    public OperateResult(bool isSuccess, string description)
    {
        IsSuccess = isSuccess;
        Description = description;
    }
    /// <summary>
    /// 成功信息
    /// </summary>
    /// <returns></returns>
    public static OperateResult DefaultSuccess => new OperateResult(true, "");
    /// <summary>
    /// 成功信息
    /// </summary>
    /// <typeparam name="S">传入类型</typeparam>
    /// <param name="content">传入内容</param>
    /// <returns></returns>
    public static OperateResult<S> SuccessResult<S>(S content) => new OperateResult<S>(true, "", content);

    /// <summary>
    /// 创建失败信息
    /// </summary>
    /// <param name="failMsg">失败描述</param>
    /// <returns></returns>
    public static OperateResult CreateFailResult(string failMsg) => new OperateResult(false, failMsg);
    /// <summary>
    /// 失败信息
    /// </summary>
    /// <typeparam name="S">失败类型</typeparam>
    /// <param name="failMsg">信息描述</param>
    /// <returns></returns>
    public static OperateResult<S> CreateFailResult<S>(string failMsg) => new OperateResult<S>(false, failMsg, default(S));
}
/// <summary>
/// 操作结果
/// </summary>
/// <typeparam name="T"></typeparam>
public struct OperateResult<T>
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccess;
    /// <summary>
    /// 描述
    /// </summary>
    public string Description;
    /// <summary>
    /// 返回结果
    /// </summary>
    public T Content;
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="isSuccess"></param>
    /// <param name="description"></param>
    /// <param name="content"></param>
    public OperateResult(bool isSuccess, string description, T content)
    {
        IsSuccess = isSuccess;
        Description = description;
        Content = content;
    }
}