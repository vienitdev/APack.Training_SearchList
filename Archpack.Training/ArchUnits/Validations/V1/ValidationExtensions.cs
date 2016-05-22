using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Archpack.Training.ArchUnits.Validations.V1
{
    /// <summary>
    /// バリデーションで使えるユーティリティ クラス。
    /// Validateで始まるメソッドはValidationResultを返します。
    /// Isで始まるメソッドはboolを返します。
    /// </summary>
    public static class ValidationExtensions
    {
        private static readonly Encoding ShiftJis = Encoding.GetEncoding("Shift_JIS");

        /// <summary>
        /// System.ComponentModel.DataAnnotations 名前空間で定義された Attribute に従ってオブジェクトの検証を行います。
        /// </summary>
        /// <param name="value">検証値</param>
        /// <param name="resumeEvaluation">検証を続行するかどうか</param>
        /// <returns>検証結果情報</returns>
        public static ValidationResult ValidateDataAnnotations(this object value, bool resumeEvaluation = true)
        {
            ValidationResult result = new ValidationResult();
            ValidationContext context = new ValidationContext(value);

            var anntationResult = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            var objectId = value.GetType().Name;
            if (!Validator.TryValidateObject(value, context, anntationResult, true))
            {
                var errors = from error in anntationResult
                             let name = string.Join(",", error.MemberNames.ToArray())
                             select new ValidationErrorInfo()
                             {
                                 Message = error.ErrorMessage,
                                 Name = name,
                                 PropertyId = name,
                                 ObjectId = objectId
                             };

                result.AddRange(errors);
            }

            return result;
        }

        /// <summary>
        /// [必須入力チェック]
        /// 値が入力されているか、チェックします。
        /// </summary>
        /// <param name="value">検証値</param>
        /// <param name="name">検証項目の名称(エラーメッセージで使用します)</param>
        /// <param name="propertyId">プログラム上のプロパティ名</param>
        /// <param name="resumeEvaluation">検証を続行するかどうか</param>
        /// <returns>検証結果情報</returns>
        public static ValidationResult ValidateRequired(this object value, string name, string propertyId, bool resumeEvaluation = true)
        {
            return Validate(value, name, propertyId, string.Format(Resources.Messages.ErrorRequired, name), value.IsRequired(), resumeEvaluation);
        }

        public static ValidationResult ValidateRequired(this Guid value, bool resumeEvaluation = true)
        {
            return Validate(value, "", "", string.Format(Resources.Messages.ErrorRequired, ""), value.IsRequired(), resumeEvaluation);
        }

        /// <summary>
        /// [必須入力チェック]
        /// 値が入力されているか、チェックします。
        /// </summary>
        /// <param name="value">検証値</param>
        /// <param name="name">検証項目の名称(エラーメッセージで使用します)</param>
        /// <param name="propertyId">プログラム上のプロパティ名</param>
        /// <param name="objectId">プログラム上のプロパティを保持するオブジェクト(クラス)名</param>
        /// <param name="resumeEvaluation">検証を続行するかどうか</param>
        /// <returns>検証結果情報</returns>
        public static ValidationResult ValidateRequired(this object value, string name, string propertyId, string objectId, bool resumeEvaluation = true)
        {
            return Validate(value, name, propertyId, objectId, string.Format(Resources.Messages.ErrorRequired, name), value.IsRequired(), resumeEvaluation);
        }

        /// <summary>
        /// [数値チェック]
        /// 0以上の数値であるか、チェックします。
        /// </summary>
        /// <param name="value">検証値</param>
        /// <param name="name">検証項目の名称(エラーメッセージで使用します)</param>
        /// <param name="propertyId">プログラム上のプロパティ名</param>
        /// <param name="resumeEvaluation">検証を続行するかどうか</param>
        /// <returns>検証結果情報</returns>
        public static ValidationResult ValidateNumber(this string value, string name, string propertyId, bool allowNegativeNumber = true, bool resumeEvaluation = true)
        {
            return Validate(value, name, propertyId, string.Format(Resources.Messages.ErrorNumberFormat, name), value.IsNumber(allowNegativeNumber), resumeEvaluation);
        }

        /// <summary>
        /// [数値チェック]
        /// 0以上の数値であるか、チェックします。
        /// </summary>
        /// <param name="value">検証値</param>
        /// <param name="name">検証項目の名称(エラーメッセージで使用します)</param>
        /// <param name="propertyId">プログラム上のプロパティ名</param>
        /// <param name="objectId">プログラム上のプロパティを保持するオブジェクト(クラス)名</param>
        /// <param name="resumeEvaluation">検証を続行するかどうか</param>
        /// <returns>検証結果情報</returns>
        public static ValidationResult ValidateNumber(this string value, string name, string propertyId, string objectId, bool allowNegativeNumber = true, bool resumeEvaluation = true)
        {
            return Validate(value, name, propertyId, objectId, string.Format(Resources.Messages.ErrorNumberFormat, name), value.IsNumber(allowNegativeNumber), resumeEvaluation);
        }

        /// <summary>
        /// [半角英数字チェック]
        /// 半角英数字のみか、チェックします。
        /// </summary>
        /// <param name="value">検証値</param>
        /// <param name="name">検証項目の名称(エラーメッセージで使用します)</param>
        /// <param name="propertyId">プログラム上のプロパティ名</param>
        /// <param name="resumeEvaluation">検証を続行するかどうか</param>
        /// <returns>検証結果情報</returns>
        public static ValidationResult ValidateAlphabetOrNumeric(this string value, string name, string propertyId, bool resumeEvaluation = true)
        {
            return Validate(value, name, propertyId, string.Format(Resources.Messages.ErrorHalfAlphaNumberFormat, name), value.IsAlphaNumeric(), resumeEvaluation);
        }

        /// <summary>
        /// [半角英数字チェック]
        /// 半角英数字のみか、チェックします。
        /// </summary>
        /// <param name="value">検証値</param>
        /// <param name="name">検証項目の名称(エラーメッセージで使用します)</param>
        /// <param name="propertyId">プログラム上のプロパティ名</param>
        /// <param name="objectId">プログラム上のプロパティを保持するオブジェクト(クラス)名</param>
        /// <param name="resumeEvaluation">検証を続行するかどうか</param>
        /// <returns>検証結果情報</returns>
        public static ValidationResult ValidateAlphabetOrNumeric(this string value, string name, string propertyId, string objectId, bool resumeEvaluation = true)
        {
            return Validate(value, name, propertyId, objectId, string.Format(Resources.Messages.ErrorHalfAlphaNumberFormat, name), value.IsAlphaNumeric(), resumeEvaluation);
        }


        /// <summary>
        /// [半角英数記号チェック]
        /// 半角英数記号のみか、チェックします。(記号は-.@)
        /// </summary>
        /// <param name="value">検証値</param>
        /// <param name="name">検証項目の名称(エラーメッセージで使用します)</param>
        /// <param name="propertyId">プログラム上のプロパティ名</param>
        /// <param name="resumeEvaluation">検証を続行するかどうか</param>
        /// <returns>検証結果情報</returns>
        public static ValidationResult ValidateAlphaNumericSymbol(this string value, string name, string propertyId, bool resumeEvaluation = true)
        {
            return Validate(value, name, propertyId, string.Format(Resources.Messages.ErrorAlphaNumberSymbolFormat, name), value.IsAlphaNumericSymbol(), resumeEvaluation);
        }

        /// <summary>
        /// [半角英数記号チェック]
        /// 半角英数記号のみか、チェックします。(記号は-.@)
        /// </summary>
        /// <param name="value">検証値</param>
        /// <param name="name">検証項目の名称(エラーメッセージで使用します)</param>
        /// <param name="propertyId">プログラム上のプロパティ名</param>
        /// <param name="objectId">プログラム上のプロパティを保持するオブジェクト(クラス)名</param>
        /// <param name="resumeEvaluation">検証を続行するかどうか</param>
        /// <returns>検証結果情報</returns>
        public static ValidationResult ValidateAlphaNumericSymbol(this string value, string name, string propertyId, string objectId, bool resumeEvaluation = true)
        {
            return Validate(value, name, propertyId, objectId, string.Format(Resources.Messages.ErrorAlphaNumberSymbolFormat, name), value.IsAlphaNumericSymbol(), resumeEvaluation);
        }

        /// <summary>
        /// [全角文字チェック]
        /// 全角文字のみか、チェックします。
        /// </summary>
        /// <param name="value">検証値</param>
        /// <param name="name">検証項目の名称(エラーメッセージで使用します)</param>
        /// <param name="propertyId">プログラム上のプロパティ名</param>
        /// <param name="resumeEvaluation">検証を続行するかどうか</param>
        /// <returns>検証結果情報</returns>
        public static ValidationResult ValidateWideString(this string value, string name, string propertyId, bool resumeEvaluation = true)
        {
            return Validate(value, name, propertyId, string.Format(Resources.Messages.ErrorWideStringFormat, name), value.IsFullWidth(), resumeEvaluation);
        }

        /// <summary>
        /// [全角文字チェック]
        /// 全角文字のみか、チェックします。
        /// </summary>
        /// <param name="value">検証値</param>
        /// <param name="name">検証項目の名称(エラーメッセージで使用します)</param>
        /// <param name="propertyId">プログラム上のプロパティ名</param>
        /// <param name="objectId">プログラム上のプロパティを保持するオブジェクト(クラス)名</param>
        /// <param name="resumeEvaluation">検証を続行するかどうか</param>
        /// <returns>検証結果情報</returns>
        public static ValidationResult ValidateWideString(this string value, string name, string propertyId, string objectId, bool resumeEvaluation = true)
        {
            return Validate(value, name, propertyId, objectId, string.Format(Resources.Messages.ErrorWideStringFormat, name), value.IsFullWidth(), resumeEvaluation);
        }

        /// <summary>
        /// [半角文字チェック]
        /// 半角文字のみか、チェックします。
        /// </summary>
        /// <param name="value">検証値</param>
        /// <param name="name">検証項目の名称(エラーメッセージで使用します)</param>
        /// <param name="propertyId">プログラム上のプロパティ名</param>
        /// <param name="resumeEvaluation">検証を続行するかどうか</param>
        /// <returns>検証結果情報</returns>
        public static ValidationResult ValidateNarrowString(this string value, string name, string propertyId, bool resumeEvaluation = true)
        {
            return Validate(value, name, propertyId, string.Format(Resources.Messages.ErrorHalfStringFormat, name), value.IsHalfWidth(), resumeEvaluation);
        }

        /// <summary>
        /// [半角文字チェック]
        /// 半角文字のみか、チェックします。
        /// </summary>
        /// <param name="value">検証値</param>
        /// <param name="name">検証項目の名称(エラーメッセージで使用します)</param>
        /// <param name="objectId"></param>
        /// <param name="propertyId">プログラム上のプロパティ名</param>
        /// <param name="objectId">プログラム上のプロパティを保持するオブジェクト(クラス)名</param>
        /// <param name="resumeEvaluation">検証を続行するかどうか</param>
        /// <returns>検証結果情報</returns>
        public static ValidationResult ValidateNarrowString(this string value, string name, string propertyId, string objectId, bool resumeEvaluation = true)
        {
            return Validate(value, name, propertyId, objectId, string.Format(Resources.Messages.ErrorHalfStringFormat, name), value.IsHalfWidth(), resumeEvaluation);
        }


        /// <summary>
        /// [最小桁数チェック]
        /// 指定された最小桁数以上か、チェックします。
        /// </summary>
        /// <param name="value">検証値</param>
        /// <param name="minDigit">最小桁数</param>
        /// <param name="name">検証項目の名称(エラーメッセージで使用します)</param>
        /// <param name="propertyId">プログラム上のプロパティ名</param>
        /// <param name="resumeEvaluation">検証を続行するかどうか</param>
        /// <returns>検証結果情報</returns>
        public static ValidationResult ValidateMinLength(this string value, int minDigit, string name, string propertyId, bool resumeEvaluation = true)
        {
            return Validate(value, name, propertyId, string.Format(Resources.Messages.ErrorMinLength, name, minDigit), value.IsDigitsMore(minDigit), resumeEvaluation);
        }

        /// <summary>
        /// [最小桁数チェック]
        /// 指定された最小桁数以上か、チェックします。
        /// </summary>
        /// <param name="value">検証値</param>
        /// <param name="minDigit">最小桁数</param>
        /// <param name="name">検証項目の名称(エラーメッセージで使用します)</param>
        /// <param name="propertyId">プログラム上のプロパティ名</param>
        /// <param name="objectId">プログラム上のプロパティを保持するオブジェクト(クラス)名</param>
        /// <param name="resumeEvaluation">検証を続行するかどうか</param>
        /// <returns>検証結果情報</returns>
        public static ValidationResult ValidateMinLength(this string value, int minDigit, string name, string propertyId, string objectId, bool resumeEvaluation = true)
        {
            return Validate(value, name, propertyId, objectId, string.Format(Resources.Messages.ErrorMinLength, name, minDigit), value.IsDigitsMore(minDigit), resumeEvaluation);
        }

        /// <summary>
        /// [最大桁数チェック]
        /// 指定された最大桁数以下か、チェックします。
        /// </summary>
        /// <param name="value">検証値</param>
        /// <param name="maxDigit">最大桁数</param>
        /// <param name="name">検証項目の名称(エラーメッセージで使用します)</param>
        /// <param name="propertyId">プログラム上のプロパティ名</param>
        /// <param name="resumeEvaluation">検証を続行するかどうか</param>
        /// <returns>検証結果情報</returns>
        public static ValidationResult ValidateMaxLength(this string value, int maxDigit, string name, string propertyId, bool resumeEvaluation = true)
        {
            return Validate(value, name, propertyId, string.Format(Resources.Messages.ErrorMaxLength, name, maxDigit), value.IsDigitsBelow(maxDigit), resumeEvaluation);
        }

        /// <summary>
        /// [最大桁数チェック]
        /// 指定された最大桁数以下か、チェックします。
        /// </summary>
        /// <param name="value">検証値</param>
        /// <param name="maxDigit">最大桁数</param>
        /// <param name="name">検証項目の名称(エラーメッセージで使用します)</param>
        /// <param name="propertyId">プログラム上のプロパティ名</param>
        /// <param name="objectId">プログラム上のプロパティを保持するオブジェクト(クラス)名</param>
        /// <param name="resumeEvaluation">検証を続行するかどうか</param>
        /// <returns>検証結果情報</returns>
        public static ValidationResult ValidateMaxLength(this string value, int maxDigit, string name, string propertyId, string objectId, bool resumeEvaluation = true)
        {
            return Validate(value, name, propertyId, objectId, string.Format(Resources.Messages.ErrorMaxLength, name, maxDigit), value.IsDigitsBelow(maxDigit), resumeEvaluation);
        }
        /// <summary>
        /// 指定された文字列がShift-JISエンコードで、指定されたバイト数以下かどうかをチェックします。
        /// </summary>
        /// <param name="value">検証値</param>
        /// <param name="length">最大バイト数</param>
        /// <param name="name">検証項目の名称(エラーメッセージで使用します)</param>
        /// <param name="propertyId">プログラム上のプロパティ名</param>
        /// <param name="resumeEvaluation">検証を続行するかどうか</param>
        /// <returns>検証結果情報</returns>
        public static ValidationResult ValidateMaxSJisByteLength(this string value, int length, string name, string propertyId, bool resumeEvaluation = true)
        {
            var target = value ?? "";
            var result = ShiftJis.GetByteCount(target) <= length;
            return Validate(value, name, propertyId, string.Format(Resources.Messages.ErrorMaxByteLength, name, length), result, resumeEvaluation);
        }
        /// <summary>
        /// 指定された文字列がShift-JISエンコードで、指定されたバイト数以下かどうかをチェックします。
        /// </summary>
        /// <param name="value">検証値</param>
        /// <param name="length">最大バイト数</param>
        /// <param name="name">検証項目の名称(エラーメッセージで使用します)</param>
        /// <param name="propertyId">プログラム上のプロパティ名</param>
        /// <param name="objectId">プログラム上のプロパティを保持するオブジェクト(クラス)名</param>
        /// <param name="resumeEvaluation">検証を続行するかどうか</param>
        /// <returns>検証結果情報</returns>
        public static ValidationResult ValidateMaxSJisByteLength(this string value, int length, string name, string propertyId, string objectId, bool resumeEvaluation = true)
        {
            var target = value ?? "";
            var result = ShiftJis.GetByteCount(target) <= length;
            return Validate(value, name, propertyId, objectId, string.Format(Resources.Messages.ErrorMaxByteLength, name, length), result, resumeEvaluation);
        }

        /// <summary>
        /// [最大値チェック]
        /// 指定された最大値以下か、チェックします。
        /// </summary>
        /// <param name="value">検証値</param>
        /// <param name="maxValue">最大値</param>
        /// <param name="name">検証項目の名称(エラーメッセージで使用します)</param>
        /// <param name="propertyId">プログラム上のプロパティ名</param>
        /// <param name="resumeEvaluation">検証を続行するかどうか</param>
        /// <returns>検証結果情報</returns>
        public static ValidationResult ValidateMaximum(this object value, decimal maxValue, string name, string propertyId, bool resumeEvaluation = true)
        {
            return Validate(value, name, propertyId, string.Format(Resources.Messages.ErrorMaxValue, name, maxValue), value.IsMaxValue(maxValue), resumeEvaluation);
        }

        /// <summary>
        /// [最大値チェック]
        /// 指定された最大値以下か、チェックします。
        /// </summary>
        /// <param name="value">検証値</param>
        /// <param name="maxValue">最大値</param>
        /// <param name="name">検証項目の名称(エラーメッセージで使用します)</param>
        /// <param name="propertyId">プログラム上のプロパティ名</param>
        /// <param name="objectId">プログラム上のプロパティを保持するオブジェクト(クラス)名</param>
        /// <param name="resumeEvaluation">検証を続行するかどうか</param>
        /// <returns>検証結果情報</returns>
        public static ValidationResult ValidateMaximum(this object value, decimal maxValue, string name, string propertyId, string objectId, bool resumeEvaluation = true)
        {
            return Validate(value, name, propertyId, objectId, string.Format(Resources.Messages.ErrorMaxValue, name, maxValue), value.IsMaxValue(maxValue), resumeEvaluation);
        }


        /// <summary>
        /// [最小値チェック]
        /// 指定された最小値以上か、チェックします。
        /// </summary>
        /// <param name="value">検証値</param>
        /// <param name="minValue">最小値</param>
        /// <param name="name">検証項目の名称(エラーメッセージで使用します)</param>
        /// <param name="propertyId">プログラム上のプロパティ名</param>
        /// <param name="resumeEvaluation">検証を続行するかどうか</param>
        /// <returns>検証結果情報</returns>
        public static ValidationResult ValidateMinimum(this object value, decimal minValue, string name, string propertyId, bool resumeEvaluation = true)
        {
            return Validate(value, name, propertyId, string.Format(Resources.Messages.ErrorMinValue, name, minValue), value.IsMinValue(minValue), resumeEvaluation);
        }

        /// <summary>
        /// [最小値チェック]
        /// 指定された最小値以上か、チェックします。
        /// </summary>
        /// <param name="value">検証値</param>
        /// <param name="minValue">最小値</param>
        /// <param name="name">検証項目の名称(エラーメッセージで使用します)</param>
        /// <param name="propertyId">プログラム上のプロパティ名</param>
        /// <param name="objectId">プログラム上のプロパティを保持するオブジェクト(クラス)名</param>
        /// <param name="resumeEvaluation">検証を続行するかどうか</param>
        /// <returns>検証結果情報</returns>
        public static ValidationResult ValidateMinimum(this object value, decimal minValue, string name, string propertyId, string objectId, bool resumeEvaluation = true)
        {
            return Validate(value, name, propertyId, objectId, string.Format(Resources.Messages.ErrorMinValue, name, minValue), value.IsMinValue(minValue), resumeEvaluation);
        }

        /// <summary>
        /// [日付書式形式チェック]
        /// 日付書式の文字列であるか、チェックします。
        /// </summary>
        /// <param name="value">検証値(日付を表す文字列)</param>
        /// <param name="format">日付のフォーマット</param>
        /// <param name="name">検証項目の名称(エラーメッセージで使用します)</param>
        /// <param name="propertyId">プログラム上のプロパティ名</param>
        /// <param name="resumeEvaluation">検証を続行するかどうか</param>
        /// <returns>検証結果情報</returns>
        public static ValidationResult ValidateDateFormat(this string value, string format, string name, string propertyId, bool resumeEvaluation = true)
        {
            return Validate(value, name, propertyId, string.Format(Resources.Messages.ErrorDateFormat, name, format), value.IsDateFormat(format), resumeEvaluation);
        }

        /// <summary>
        /// [日付書式形式チェック]
        /// 日付書式の文字列であるか、チェックします。
        /// </summary>
        /// <param name="value">検証値(日付を表す文字列)</param>
        /// <param name="format">日付のフォーマット</param>
        /// <param name="name">検証項目の名称(エラーメッセージで使用します)</param>
        /// <param name="propertyId">プログラム上のプロパティ名</param>
        /// <param name="objectId">プログラム上のプロパティを保持するオブジェクト(クラス)名</param>
        /// <param name="resumeEvaluation">検証を続行するかどうか</param>
        /// <returns>検証結果情報</returns>
        public static ValidationResult ValidateDateFormat(this string value, string format, string name, string propertyId, string objectId, bool resumeEvaluation = true)
        {
            return Validate(value, name, propertyId, objectId, string.Format(Resources.Messages.ErrorDateFormat, name, format), value.IsDateFormat(format), resumeEvaluation);
        }

        /// <summary>
        /// [メールアドレス形式チェック]
        /// メールアドレス形式か、チェックします。
        /// 検証値が設定されていない場合は、検証を行いません。
        /// </summary>
        /// <param name="value">検証値(メールアドレスを表す文字列)</param>
        /// <param name="name">検証項目の名称(エラーメッセージで使用します)</param>
        /// <param name="propertyId">プログラム上のプロパティ名</param>
        /// <param name="resumeEvaluation">検証を続行するかどうか</param>
        /// <returns>検証結果情報</returns>
        public static ValidationResult ValidateEmailAddress(this string value, string name, string propertyId, bool resumeEvaluation = true)
        {
            return Validate(value, name, propertyId, string.Format(Resources.Messages.ErrorMailAddressFormat, name), value.IsEmailAddress(), resumeEvaluation);
        }
        /// <summary>
        /// [メールアドレス形式チェック]
        /// メールアドレス形式か、チェックします。
        /// 検証値が設定されていない場合は、検証を行いません。
        /// </summary>
        /// <param name="value">検証値(メールアドレスを表す文字列)</param>
        /// <param name="name">検証項目の名称(エラーメッセージで使用します)</param>
        /// <param name="propertyId">プログラム上のプロパティ名</param>
        /// <param name="objectId">プログラム上のプロパティを保持するオブジェクト(クラス)名</param>
        /// <param name="resumeEvaluation">検証を続行するかどうか</param>
        /// <returns>検証結果情報</returns>
        public static ValidationResult ValidateEmailAddress(this string value, string name, string propertyId, string objectId, bool resumeEvaluation = true)
        {
            return Validate(value, name, propertyId, objectId, string.Format(Resources.Messages.ErrorMailAddressFormat, name), value.IsEmailAddress(), resumeEvaluation);
        }

        #region バリデーション用内部メソッド

        private static ValidationResult Validate(object value, string name, string propertyId, string message, bool success, bool resumeEvaluation)
        {
            return Validate(value, name, propertyId, null, message, success, resumeEvaluation);
        }

        private static ValidationResult Validate(object value, string name, string propertyId, string objectId, string message, bool success, bool resumeEvaluation)
        {
            ValidationResult result = new ValidationResult();

            if (!success)
            {
                ValidationErrorInfo info = new ValidationErrorInfo() { PropertyId = propertyId, Name = name, Value = value, Message = message, ObjectId = objectId };
                result.Errors.Add(info);
                result.ResumeEvaluation = resumeEvaluation;
            }

            return result;
        }

        private static bool IsRequired(this Guid value)
        {
            return value != Guid.Empty;
        }


        /// <summary>
        /// [必須入力チェック]
        /// 値が入力されているか、チェックします。
        /// </summary>
        /// <param name="value">検証値</param>
        /// <returns>入力されている場合 true / それ以外の場合 false</returns>
        private static bool IsRequired(this object value)
        {
            if (value == null)
            {
                return false;
            }

            return !string.IsNullOrEmpty(value.ToString());
        }

        /// <summary>
        /// [数値チェック]
        /// 数値文字列かチェックします。
        /// </summary>
        /// <param name="value">検証値</param>
        /// <param name="allowNegativeNumber">マイナス値許可フラグ(マイナス値を許容する場合True / それ以外の場合False)</param>
        /// <returns>数値の場合 true / それ以外の場合 false</returns>
        private static bool IsNumber(this string value, bool allowNegativeNumber)
        {
            if (string.IsNullOrEmpty(value))
            {
                return true;
            }

            if (!Regex.IsMatch(value, "^-?(([1-9][0-9]*)|(0))(\\.[0-9]+)?$"))
            {
                return false;
            }
            if (!allowNegativeNumber)
            {
                return value[0] != '-';
            }
            return true;
        }

        /// <summary>
        /// [半角英数チェック]
        /// 半角英数のみか、チェックします。
        /// 引数が設定されていない場合、falseを返します。
        /// </summary>
        /// <param name="value">検証値</param>
        /// <returns>半角英数のみの場合 true / それ以外の場合 false</returns>
        private static bool IsAlphaNumeric(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return true;
            }

            return Regex.IsMatch(value, "^[a-zA-Z0-9]+$");

        }

        /// <summary>
        /// [半角英数記号チェック(記号は-.@)]
        /// 半角英数記号のみか、チェックします。
        /// 引数が設定されていない場合、falseを返します。
        /// </summary>
        /// <param name="value">検証値</param>
        /// <returns>半角英数記号のみの場合 True / それ以外の場合 False</returns>
        private static bool IsAlphaNumericSymbol(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return true;
            }

            return Regex.IsMatch(value, "^[a-zA-Z0-9.@-]+$");
        }

        /// <summary>
        /// [全角文字チェック]
        /// 全て全角文字か、チェックします。
        /// </summary>
        /// <param name="value">検証値</param>
        /// <returns>全て全角文字の場合True / それ以外の場合False</returns>
        private static bool IsFullWidth(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return true;
            }

            foreach (char c in value)
            {
                if (c.ToString().IsHalfWidth())
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// [半角文字チェック]
        /// 全て半角文字か、チェックします。
        /// </summary>
        /// <param name="value"></param>
        /// <returns>全て半角文字の場合True / それ以外の場合False</returns>
        private static bool IsHalfWidth(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return true;
            }

            foreach (char c in value)
            {
                if (Char.IsSurrogate(c))
                {
                    return false;
                }

                if (ShiftJis.GetByteCount(value) != value.Length)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// [最小桁数チェック]
        /// 指定された桁数以上か、チェックします。
        /// 桁数がマイナス値の場合、ArgumentExceptionをthrowします。
        /// </summary>
        /// <param name="value">検証値</param>
        /// <param name="maxDigit">最小桁数</param>
        /// <returns>最小桁数以上の場合True / それ以外の場合False</returns>
        private static bool IsDigitsMore(this string value, int minDigit)
        {
            if (minDigit < 0)
            {
                throw new ArgumentException(Resources.Messages.ErrorDigitMinus, minDigit.ToString());
            }
            if (string.IsNullOrEmpty(value))
            {
                return true;
            }
            return Regex.IsMatch(value.ToString(), "^.{" + minDigit.ToString() + ",}$");
        }

        /// <summary>
        /// [最大桁数チェック]
        /// 指定された桁数以下か、チェックします。
        /// 桁数がマイナス値の場合、ArgumentExceptionをthrowします。
        /// </summary>
        /// <param name="value">検証値</param>
        /// <param name="maxDigit">最大桁数</param>
        /// <returns>最大桁数以下の場合True / それ以外の場合False</returns>
        private static bool IsDigitsBelow(this string value, int maxDigit)
        {
            if (maxDigit < 0)
            {
                throw new ArgumentException(Resources.Messages.ErrorDigitMinus, maxDigit.ToString());
            }
            if (string.IsNullOrEmpty(value))
            {
                return true;
            }
            return Regex.IsMatch(value.ToString(), "^.{0," + maxDigit.ToString() + "}$");
        }

        /// <summary>
        /// [最大値チェック]
        /// 指定された最大値以下か、チェックします。
        /// </summary>
        /// <param name="value">入力された値</param>
        /// <param name="maxValue">設定されている最大値</param>
        /// <returns>最大値以下の場合 true / それ以外の場合 false</returns>
        private static bool IsMaxValue(this object value, decimal maxValue)
        {
            if (value == null)
            {
                return true;
            }

            string input = value.ToString();

            if (string.IsNullOrEmpty(input))
            {
                return true;
            }

            if (!input.IsNumber(true))
            {
                return false;
            }

            Decimal number;
            if (Decimal.TryParse(input, out number))
            {
                if (number <= maxValue)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// [最小値チェック]
        /// 指定された最小値以上であるか、チェックします。
        /// </summary>
        /// <param name="value">入力された値</param>
        /// <param name="minValue">設定されている最小値</param>
        /// <returns>最小値以上の場合 true / それ以外の場合 false</returns>
        private static bool IsMinValue(this object value, decimal minValue)
        {
            if (value == null)
            {
                return true;
            }

            string input = value.ToString();

            if (string.IsNullOrEmpty(input))
            {
                return true;
            }

            if (!input.IsNumber(true))
            {
                return false;
            }

            Decimal number;
            if (Decimal.TryParse(input, out number))
            {
                if (number >= minValue)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 指定されたフォーマットの日付型の文字列かチェックします。
        /// </summary>
        /// <param name="value">日付を表す文字列</param>
        /// <returns>指定された日付書式の文字列の場合True / それ以外の場合False</returns>
        private static bool IsDateFormat(this string value, string format)
        {
            if (string.IsNullOrEmpty(value))
            {
                return true;
            }

            DateTime dateTime;
            if (DateTime.TryParseExact(value, format, null, DateTimeStyles.None, out dateTime))
            {
                if (dateTime < new DateTime(1753, 1, 1))
                {
                    return false;
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// [メールアドレス形式チェック]
        /// メールアドレス形式か、チェックします。
        /// 検証値が設定されていない場合、Falseを返します。
        /// </summary>
        /// <param name="value">検証値</param>
        /// <returns>メールアドレス形式の場合 true / それ以外の場合 false</returns>
        private static bool IsEmailAddress(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return true;
            }

            return Regex.IsMatch(value, "^[a-zA-Z0-9!$&*.=^`|~#%'+\\/?_{}-]+@([a-zA-Z0-9_-]+\\.)+[a-zA-Z]{1,4}$");
        }

        #endregion
    }
}