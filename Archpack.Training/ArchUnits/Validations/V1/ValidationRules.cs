using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Validations.V1
{
    /// <summary>
    /// 入力チェックのルールを管理するクラス
    /// </summary>
    public class ValidationRules
    {
        private List<Func<ValidationResult>> _rules = new List<Func<ValidationResult>>();

        /// <summary>
        /// 入力チェックのルールを追加します
        /// </summary>
        /// <param name="rule"></param>
        public void Add(Func<ValidationResult> rule)
        {
            _rules.Add(rule);
        }

        /// <summary>
        /// 追加された入力チェックを一括実行します。
        /// ResumeEvaluation(後続チェック実行フラグ)がFalseの場合、後続のチェックは行いません。
        /// </summary>
        /// <returns>検証結果情報</returns>
        public ValidationResult Validate()
        {
            ValidationResult result = new ValidationResult();

            foreach (var rule in _rules)
            {
                var ruleResult = rule.Invoke();
                result.Errors.AddRange(ruleResult.Errors);

                result.ResumeEvaluation = ruleResult.ResumeEvaluation;

                //後続チェック実行フラグがFalseの場合、以降のチェックは行わない
                if (!ruleResult.ResumeEvaluation)
                {
                    break;
                }
            }
            return result;
        }
    }

    /// <summary>
    /// 入力チェックのルールを管理するクラス
    /// </summary>
    public class ValidationRules<T>
    {
        private List<Func<T, ValidationResult>> _rules = new List<Func<T, ValidationResult>>();

        /// <summary>
        /// 入力チェックのルールを追加します
        /// </summary>
        /// <param name="rule"></param>
        public void Add(Func<T, ValidationResult> rule)
        {
            _rules.Add(rule);
        }

        /// <summary>
        /// 追加された入力チェックを一括実行します。
        /// ResumeEvaluation(後続チェック実行フラグ)がFalseの場合、後続のチェックは行いません。
        /// </summary>
        /// <returns>検証結果情報</returns>
        public ValidationResult Validate(T target)
        {
            ValidationResult result = new ValidationResult();
            var objectId = typeof(T).Name;

            foreach (var rule in _rules)
            {
                var ruleResult = rule.Invoke(target);
                result.Errors.AddRange(GetPreparedErrors(ruleResult));

                result.ResumeEvaluation = ruleResult.ResumeEvaluation;

                //後続チェック実行フラグがFalseの場合、以降のチェックは行わない
                if (!ruleResult.ResumeEvaluation)
                {
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// 追加された入力チェックを一括実行します。
        /// ResumeEvaluation(後続チェック実行フラグ)がFalseの場合、後続のチェックは行いません。
        /// </summary>
        /// <returns>検証結果情報</returns>
        public ValidationResult Validate(IEnumerable<T> targets)
        {
            ValidationResult result = new ValidationResult();

            foreach (var target in targets)
            {
                var ruleResult = Validate(target);
                result.Errors.AddRange(GetPreparedErrors(ruleResult));

                result.ResumeEvaluation = ruleResult.ResumeEvaluation;

                if (!ruleResult.ResumeEvaluation)
                {
                    break;
                }
            }

            return result;
        }
        /// <summary>
        /// バリデーション結果のエラーを整形します。
        /// </summary>
        /// <param name="result">整形するエラーをもつ <see cref="ValidationResult"/></param>
        /// <returns>整形されたバリデーションのエラー一覧</returns>
        private IEnumerable<ValidationErrorInfo> GetPreparedErrors(ValidationResult result) 
        {
            var objectId = typeof(T).Name;
            return result.Errors.Select(e => 
            {
                e.ObjectId = e.ObjectId ?? objectId;
                return e;
            });
        }
    }
}