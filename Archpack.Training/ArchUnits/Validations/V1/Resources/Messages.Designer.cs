﻿//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.42000
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Archpack.Training.ArchUnits.Validations.V1.Resources {
    using System;
    
    
    /// <summary>
    ///   ローカライズされた文字列などを検索するための、厳密に型指定されたリソース クラスです。
    /// </summary>
    // このクラスは StronglyTypedResourceBuilder クラスが ResGen
    // または Visual Studio のようなツールを使用して自動生成されました。
    // メンバーを追加または削除するには、.ResX ファイルを編集して、/str オプションと共に
    // ResGen を実行し直すか、または VS プロジェクトをビルドし直します。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Messages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Messages() {
        }
        
        /// <summary>
        ///   このクラスで使用されているキャッシュされた ResourceManager インスタンスを返します。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Archpack.Training.ArchUnits.Validations.V1.Resources.Messages", typeof(Messages).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   厳密に型指定されたこのリソース クラスを使用して、すべての検索リソースに対し、
        ///   現在のスレッドの CurrentUICulture プロパティをオーバーライドします。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   {0}は半角英数記号のみで入力して下さい。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ErrorAlphaNumberSymbolFormat {
            get {
                return ResourceManager.GetString("ErrorAlphaNumberSymbolFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   {0}は{1}形式で入力してください。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ErrorDateFormat {
            get {
                return ResourceManager.GetString("ErrorDateFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   桁数にマイナスは指定できません。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ErrorDigitMinus {
            get {
                return ResourceManager.GetString("ErrorDigitMinus", resourceCulture);
            }
        }
        
        /// <summary>
        ///   {0}は半角英数字のみで入力してください。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ErrorHalfAlphaNumberFormat {
            get {
                return ResourceManager.GetString("ErrorHalfAlphaNumberFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   {0}は半角文字で入力してください。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ErrorHalfStringFormat {
            get {
                return ResourceManager.GetString("ErrorHalfStringFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   {0}はメールアドレスを入力してください。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ErrorMailAddressFormat {
            get {
                return ResourceManager.GetString("ErrorMailAddressFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   {0}は{1}バイト以下で入力してください。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ErrorMaxByteLength {
            get {
                return ResourceManager.GetString("ErrorMaxByteLength", resourceCulture);
            }
        }
        
        /// <summary>
        ///   {0}は{1}文字以下で入力してください。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ErrorMaxLength {
            get {
                return ResourceManager.GetString("ErrorMaxLength", resourceCulture);
            }
        }
        
        /// <summary>
        ///   {0}は{1}以下の値を入力してください。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ErrorMaxValue {
            get {
                return ResourceManager.GetString("ErrorMaxValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   エラーが発生しました。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ErrorMessageTitle {
            get {
                return ResourceManager.GetString("ErrorMessageTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   {0}は{1}文字以上で入力してください。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ErrorMinLength {
            get {
                return ResourceManager.GetString("ErrorMinLength", resourceCulture);
            }
        }
        
        /// <summary>
        ///   {0}は{1}以上の値を入力してください。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ErrorMinValue {
            get {
                return ResourceManager.GetString("ErrorMinValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   {0}は数値で入力してください。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ErrorNumberFormat {
            get {
                return ResourceManager.GetString("ErrorNumberFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   {0}は必須入力です。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ErrorRequired {
            get {
                return ResourceManager.GetString("ErrorRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   {0}は全角文字で入力してください。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ErrorWideStringFormat {
            get {
                return ResourceManager.GetString("ErrorWideStringFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   ファイル ({0}) が見つかりません。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string FileNotExistsErrorFormat {
            get {
                return ResourceManager.GetString("FileNotExistsErrorFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   ユーザー {0} は既に他のユーザーによって変更されています。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string UserUpdateConflict {
            get {
                return ResourceManager.GetString("UserUpdateConflict", resourceCulture);
            }
        }
        
        /// <summary>
        ///   {0}は必ず入力してください。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ValidationRequired {
            get {
                return ResourceManager.GetString("ValidationRequired", resourceCulture);
            }
        }
    }
}