using System;

namespace Archpack.Training.ArchUnits.Pipeline.V1
{
    /// <summary>
    /// パイプアクションのインターフェース。
    /// </summary>
    public interface IPipeAction
    {
        /// <summary>
        /// 指定されたアクション（メソッド）をパイプライン処理として実行します。
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        PipeResponse Execute(PipeRequest request);

        bool IsResumePipe { get; }

        Action OnStart { get; set; }

        Action OnEnd { get; set; }

        Action OnError { get; set; }
    }
}