using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Validations.V1
{
    /// <summary>
    /// 入力検証結果情報を管理するクラスです。
    /// </summary>
    public class ValidationResult
    {
        private List<ValidationErrorInfo> _errors = new List<ValidationErrorInfo>();

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public ValidationResult()
        {
            //後続チェック実行フラグの初期値をTrue(後続チェック実行する)にします。
            this.ResumeEvaluation = true;
        }

        /// <summary>
        /// 検証の結果がエラーかどうかを返します。
        /// </summary>
        /// <returns>エラー情報が1件も存在しない場合は True、それ以外は False</returns>
        public bool IsValid
        {
            get { return _errors.Count == 0; }
        }

        /// <summary>
        /// エラー情報を取得します。
        /// </summary>
        /// <returns>エラー情報</returns>
        public List<ValidationErrorInfo> Errors
        {
            get { return _errors; }
        }

        /// <summary>
        /// エラー詳細情報リストの各要素をエラー情報の末尾に追加します。
        /// </summary>
        /// <param name="errors"></param>
        public ValidationResult Chain(ValidationResult target)
        {
            _errors.AddRange(target.Errors);

            return this;
        }

        /// <summary>
        /// エラー詳細情報リストの各要素をエラー情報の末尾に追加します。
        /// </summary>
        /// <param name="errors"></param>
        public void AddRange(IEnumerable<ValidationErrorInfo> errors)
        {
            _errors.AddRange(errors);
        }

        /// <summary>
        /// 後続チェック実行フラグを取得または設定します。
        /// </summary>
        public bool ResumeEvaluation { get; set; }
    }

    /// <summary>
    /// 入力検証エラー情報クラスです。
    /// </summary>
    public class ValidationErrorInfo
    {
        /// <summary>
        /// エラーが発生したプロパティを保持するオブジェクトのプログラム上のIDを格納します。
        /// </summary>
        public string ObjectId { get; set; }

        /// <summary>
        /// エラーが発生したプロパティのプログラム上のIDを格納します。
        /// </summary>
        public string PropertyId { get; set; }

        /// <summary>
        /// エラーが発生したプロパティのデータを格納します。
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// エラーが発生したプロパティの表示名を格納します。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// エラーが発生したプロパティの値を格納します。
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// エラー内容を示すメッセージを表示します。
        /// </summary>
        public string Message { get; set; }
    }
}