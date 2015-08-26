using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Nikse.SubtitleEdit.Core
{
    /// <summary>
    /// Various language identifiers for subsystems like Hunspell, Tesseract and Google Translate API
    /// </summary>
    public sealed class LanguageInfo: IEquatable<LanguageInfo>, IComparable<LanguageInfo>
    {
        private static readonly LanguagesByName SupportedLanguages = new LanguagesByName(new[]
        {
            // LanguageInfo(Name, ThreeLetterIsoName, EnglishName, [FullName], [GoogleName], [HunspellName], [TesseractName])
            //
            // Name: <language>[-<script>][-<region>] (≡ .NET CultureInfo.Name)
            //    http://www.unicode.org/reports/tr35/tr35.html#Unicode_Language_and_Locale_Identifiers
            //
            // ThreeLetterIsoName: ISO 639-2 Code (≡ .NET CultureInfo.ThreeLetterIsoName)
            //    (https://www.loc.gov/standards/iso639-2/php/code_list.php)
            //
            // EnglishName: CultureInfo.GetCultureInfo(Name).EnglishName
            //    or look at http://www.unicode.org/cldr/charts/latest/summary/en.html
            //    or core/common/main/en.xml (http://cldr.unicode.org/index/downloads)
            //
            // FullName: <language>[-<script>]_<region> (territory locale)
            //    CultureInfo.CreateSpecificCulture(Name).Name
            //    with an underscore before <region> instead of a hyphen.
            //    Do NOT specify if (FullName.Replace('_', '-') == Name)
            //
            // GoogleName: Google Translate API language identifier (if supported)
            //
            // HunspellName: dictionary file name without extension (if supported)
            //
            // TesseractName: language identifier (if supported)
            //
            new LanguageInfo("af", "afr", "Afrikaans", "af_ZA", GoogleName: "af", HunspellName: "af_ZA"),
            //new LanguageInfo("af-ZA", "afr", "Afrikaans (South Africa)", HunspellName: "af_ZA"),
            new LanguageInfo("ach", "ach", "Acholi", "ach_UG"),
            //new LanguageInfo("ach-UG", "ach", "Acholi (Uganda)", HunspellName: "ach_UG ?"),
            new LanguageInfo("ak", "aka", "Akan", "ak_GH", HunspellName: "ak_GH"),
            //new LanguageInfo("ak-GH", "aka", "Akan (Ghana)", HunspellName: "ak_GH"),
            new LanguageInfo("am", "amh", "Amharic", "am_ET", HunspellName: "am_ET"),
            //new LanguageInfo("am-ET", "amh", "Amharic (Ethiopia)", HunspellName: "am_ET"),
            new LanguageInfo("an", "arg", "Aragonese", "an_ES", HunspellName: "an_ES"),
            //new LanguageInfo("an-ES", "arg", "Aragonese (Spain)", HunspellName: "an_ES"),
            new LanguageInfo("ar", "ara", "Arabic", "ar_EG", GoogleName: "ar", HunspellName: "ar"),
            //new LanguageInfo("ar-001", "ara", "Arabic - Modern Standard Arabic", HunspellName: "ar_001 ?"),
            //new LanguageInfo("ar-AE", "ara", "Arabic (U.A.E.)", HunspellName: "ar_AE ?"),
            //new LanguageInfo("ar-BH", "ara", "Arabic (Bahrain)", HunspellName: "ar_BH ?"),
            //new LanguageInfo("ar-DZ", "ara", "Arabic (Algeria)", HunspellName: "ar_DZ ?"),
            //new LanguageInfo("ar-EG", "ara", "Arabic (Egypt)", HunspellName: "ar_EG ?"),
            //new LanguageInfo("ar-IQ", "ara", "Arabic (Iraq)", HunspellName: "ar_IQ ?"),
            //new LanguageInfo("ar-JO", "ara", "Arabic (Jordan)", HunspellName: "ar_JO ?"),
            //new LanguageInfo("ar-KW", "ara", "Arabic (Kuwait)", HunspellName: "ar_KW ?"),
            //new LanguageInfo("ar-LB", "ara", "Arabic (Lebanon)", HunspellName: "ar_LB ?"),
            //new LanguageInfo("ar-LY", "ara", "Arabic (Libya)", HunspellName: "ar_LY ?"),
            //new LanguageInfo("ar-MA", "ara", "Arabic (Morocco)", HunspellName: "ar_MA ?"),
            //new LanguageInfo("ar-OM", "ara", "Arabic (Oman)", HunspellName: "ar_OM ?"),
            //new LanguageInfo("ar-QA", "ara", "Arabic (Qatar)", HunspellName: "ar_QA ?"),
            //new LanguageInfo("ar-SA", "ara", "Arabic (Saudi Arabia)", HunspellName: "ar_SA ?"),
            //new LanguageInfo("ar-SD", "ara", "Arabic (Sudan)", HunspellName: "ar_SD ?"),
            //new LanguageInfo("ar-SY", "ara", "Arabic (Syria)", HunspellName: "ar_SY ?"),
            //new LanguageInfo("ar-TN", "ara", "Arabic (Tunisia)", HunspellName: "ar_TN ?"),
            //new LanguageInfo("ar-YE", "ara", "Arabic (Yemen)", HunspellName: "ar_YE ?"),
            new LanguageInfo("arn", "arn", "Mapudungun", "arn_CL"),
            //new LanguageInfo("arn-CL", "arn", "Mapudungun (Chile)", HunspellName: "arn_CL ?"),
            new LanguageInfo("as", "asm", "Assamese", "as_IN", HunspellName: "as-IN"),
            //new LanguageInfo("as-IN", "asm", "Assamese (India)", HunspellName: "as-IN"),
            new LanguageInfo("ast", "ast", "Asturian", "ast_ES", HunspellName: "ast"),
            //new LanguageInfo("ast-ES", "ast", "Asturian (Spain)", HunspellName: "ast"),
            new LanguageInfo("az-Arab", "aze", "Azeri (Arabic)", "az-Arab_IR"),
            //new LanguageInfo("az-Arab-IR", "aze", "Azeri (Arabic, Iran)", HunspellName: "az-Arab-IR ?"),
            new LanguageInfo("az-Cyrl", "aze", "Azeri (Cyrillic)", "az-Cyrl_RU"),
            //new LanguageInfo("az-Cyrl-AZ", "aze", "Azeri (Cyrillic, Azerbaijan)", HunspellName: "az-Cyrl-AZ ?"),
            //new LanguageInfo("az-Cyrl-RU", "aze", "Azeri (Cyrillic, Russia)", HunspellName: "az-Cyrl-AZ ?"),
            new LanguageInfo("az-Latn", "aze", "Azeri (Latin)", "az-Latn_AZ", GoogleName: "az", HunspellName: "az-Latn-AZ"),
            //new LanguageInfo("az-Latn-AZ", "aze", "Azeri (Latin, Azerbaijan)", HunspellName: "az-Latn-AZ"),
            new LanguageInfo("ba", "bak", "Bashkir", "ba_RU"),
            //new LanguageInfo("ba-RU", "bak", "Bashkir (Russia)", HunspellName: "ba_RU ?"),
            new LanguageInfo("bas", "bas", "Basaa", "bas_CM"),
            //new LanguageInfo("bas-CM", "bas", "Basaa (Cameroon)", HunspellName: "bas_CM ?"),
            new LanguageInfo("be", "bel", "Belarusian", "be_BY", GoogleName: "be", HunspellName: "be"),
            //new LanguageInfo("be:modern", "bel", "Belarusian", "be_BY", HunspellName: "be"),
            //new LanguageInfo("be:classic", "bel", "Belarusian", "be_BY", HunspellName: "be-classic"),
            //new LanguageInfo("be-BY", "bel", "Belarusian (Belarus)", HunspellName: "be"),
            //new LanguageInfo("be-BY:modern", "bel", "Belarusian (Belarus)", HunspellName: "be"),
            //new LanguageInfo("be-BY:classic", "bel", "Belarusian (Belarus)", HunspellName: "be-classic"),
            new LanguageInfo("bem", "bem", "Bemba", "bem_ZM"),
            //new LanguageInfo("bem-ZM", "bem", "Bemba (Zambia)", HunspellName: "bem_ZM ?"),
            new LanguageInfo("bg", "bul", "Bulgarian", "bg_BG", GoogleName: "bg", HunspellName: "bg"),
            //new LanguageInfo("bg-BG", "bul", "Bulgarian (Bulgaria)", HunspellName: "bg"),
            new LanguageInfo("bm", "bam", "Bambara", "bm_ML"),
            //new LanguageInfo("bm-ML", "bam", "Bambara (Mali)", HunspellName: "bm_ML ?"),
            new LanguageInfo("bn", "ben", "Bengali", "bn_BD", GoogleName: "bn", HunspellName: "bn-BD"),
            //new LanguageInfo("bn-BD", "ben", "Bengali (Bangladesh)", HunspellName: "bn-BD"),
            //new LanguageInfo("bn-IN", "ben", "Bengali (India)", HunspellName: "bn-IN ?"),
            new LanguageInfo("bo", "bod", "Tibetan", "bo_CN"),
            //new LanguageInfo("bo-CN", "bod", "Tibetan (China)", HunspellName: "bo_CN ?"),
            //new LanguageInfo("bo-IN", "bod", "Tibetan (India)", HunspellName: "bo_IN ?"),
            new LanguageInfo("br", "bre", "Breton", "br_FR", HunspellName: "br"),
            //new LanguageInfo("br-FR", "bre", "Breton (France)", HunspellName: "br"),
            new LanguageInfo("bs-Cyrl", "bsc", "Bosnian (Cyrillic)", "bs-Cyrl_BA"),
            //new LanguageInfo("bs-Cyrl-BA", "bsc", "Bosnian (Cyrillic, Bosnia and Herzegovina)", HunspellName: "bs_Cyrl_BA ?"),
            new LanguageInfo("bs-Latn", "bsb", "Bosnian (Latin)", "bs-Latn_BA", GoogleName: "bs"),
            //new LanguageInfo("bs-Latn-BA", "bsb", "Bosnian (Latin, Bosnia and Herzegovina)", HunspellName: "bs_Latn_BA ?"),
            new LanguageInfo("ca", "cat", "Catalan", "ca_ES", GoogleName: "ca", HunspellName: "ca"), // IEC (Institut d'Estudis Catalans)
            new LanguageInfo("ca:valencia [AVL]", "cat", "Catalan", "ca_ES", HunspellName: "ca-ES-valencia"), // AVL (Acadèmia Valenciana de la Llengua)
            new LanguageInfo("ca:valencia [RACV]", "cat", "Catalan", "ca_ES", HunspellName: "roa-ES-val"),    // RACV (Real Acadèmia de Cultura Valenciana)
            //new LanguageInfo("ca-AN", "cat", "Catalan (Andorra)", HunspellName: "ca"),
            //new LanguageInfo("ca-ES", "cat", "Catalan (Catalonia)", HunspellName: "ca"), // IEC (Institut d'Estudis Catalans)
            //new LanguageInfo("ca-ES:valencia [AVL]", "cat", "Catalan", "ca_ES", HunspellName: "ca-ES-valencia"), // AVL (Acadèmia Valenciana de la Llengua)
            //new LanguageInfo("ca-ES:valencia [RACV]", "cat", "Catalan", "ca_ES", HunspellName: "roa-ES-val"),    // RACV (Real Acadèmia de Cultura Valenciana)
            new LanguageInfo("ceb", "ceb", "Cebuano", "ceb_PH", GoogleName: "ceb"),
            new LanguageInfo("chr", "chr", "Cherokee", "chr_US"),
            //new LanguageInfo("chr-US", "chr", "Cherokee (United States)", HunspellName: "chr_US ?"),
            new LanguageInfo("co", "cos", "Corsican", "co_FR"),
            //new LanguageInfo("co-FR", "cos", "Corsican (France)", HunspellName: "co_FR ?"),
            new LanguageInfo("cs", "ces", "Czech", "cs_CZ", GoogleName: "cs", HunspellName: "cs_CZ"),
            //new LanguageInfo("cs-CZ", "ces", "Czech (Czech Republic)", HunspellName: "cs"),
            new LanguageInfo("cy", "cym", "Welsh", "cy_GB", GoogleName: "cy"),
            //new LanguageInfo("cy-GB", "cym", "Welsh (United Kingdom)", HunspellName: "cy_GB ?"),
            new LanguageInfo("da", "dan", "Danish", "da_DK", GoogleName: "da", HunspellName: "da_DK"),
            //new LanguageInfo("da-DK", "dan", "Danish (Denmark)", HunspellName: "da_DK"),
            new LanguageInfo("de", "deu", "German", "de_DE", GoogleName: "de"),
            new LanguageInfo("de-AT", "deu", "German (Austria)", HunspellName: "de_AT"),
            new LanguageInfo("de-CH", "deu", "German (Switzerland)", HunspellName: "de_CH"),
            new LanguageInfo("de-DE", "deu", "German (Germany)", HunspellName: "de_DE"),
            //new LanguageInfo("de-LI", "deu", "German (Liechtenstein)", HunspellName: "de_LI"),
            //new LanguageInfo("de-LU", "deu", "German (Luxembourg)", HunspellName: "de_LU"),
            new LanguageInfo("dsb", "dsb", "Lower Sorbian", "dsb_DE"),
            //new LanguageInfo("dsb-DE", "dsb", "Lower Sorbian (Germany)", HunspellName: "dsb_DE"),
            new LanguageInfo("hsb", "hsb", "Upper Sorbian", "hsb_DE"),
            //new LanguageInfo("hsb-DE", "hsb", "Upper Sorbian (Germany)", HunspellName: "hsb_DE"),
            new LanguageInfo("dv", "div", "Divehi", "dv_MV"),
            //new LanguageInfo("dv-MV", "div", "Divehi (Maldives)", HunspellName: "dv_MV ?"),
            new LanguageInfo("ee", "ewe", "Ewe", "ee_GH", HunspellName: "ee-GH"),
            //new LanguageInfo("ee-GH", "ewe", "Ewe (Ghana)", HunspellName: "ee-GH"),
            //new LanguageInfo("ee-TG", "ewe", "Ewe (Togo)", HunspellName: "ee-TG ?"),
            new LanguageInfo("el", "ell", "Greek", "el_GR", GoogleName: "el", HunspellName: "el-GR"),
            //new LanguageInfo("el-GR", "ell", "Greek (Greece)", HunspellName: "el-GR"),
            new LanguageInfo("en", "eng", "English", "en_US", GoogleName: "en"),
            //new LanguageInfo("en-029", "eng", "English (Caribbean)", HunspellName: "en_029 ?"),
            //new LanguageInfo("en-AS", "eng", "English (American Samoa)", HunspellName: "en_AS ?"),
            new LanguageInfo("en-AU", "eng", "English (Australia)", HunspellName: "en_AU"),
            //new LanguageInfo("en-BB", "eng", "English (Barbados)", HunspellName: "en_BB ?"),
            //new LanguageInfo("en-BE", "eng", "English (Belgium)", HunspellName: "en_BE ?"),
            //new LanguageInfo("en-BM", "eng", "English (Bermuda)", HunspellName: "en_BM ?"),
            //new LanguageInfo("en-BW", "eng", "English (Botswana)", HunspellName: "en_BW ?"),
            //new LanguageInfo("en-BZ", "eng", "English (Belize)", HunspellName: "en_BZ ?"),
            new LanguageInfo("en-CA", "eng", "English (Canada)", HunspellName: "en_CA"),
            new LanguageInfo("en-GB", "eng", "English (United Kingdom)", HunspellName: "en_GB"),
            //new LanguageInfo("en-GU", "eng", "English (Guam)", HunspellName: "en_GU ?"),
            //new LanguageInfo("en-GY", "eng", "English (Guyana)", HunspellName: "en_GY ?"),
            //new LanguageInfo("en-HK", "eng", "English (Hong Kong)", HunspellName: "en_HK ?"),
            //new LanguageInfo("en-IE", "eng", "English (Ireland)", HunspellName: "en_IE ?"),
            //new LanguageInfo("en-IN", "eng", "English (India)", HunspellName: "en_IN ?"),
            //new LanguageInfo("en-JM", "eng", "English (Jamaica)", HunspellName: "en_JM ?"),
            //new LanguageInfo("en-MH", "eng", "English (Marshall Islands)", HunspellName: "en_MH ?"),
            //new LanguageInfo("en-MP", "eng", "English (Mariana Islands)", HunspellName: "en_MP ?"),
            //new LanguageInfo("en-MT", "eng", "English (Malta)", HunspellName: "en_MT ?"),
            //new LanguageInfo("en-MU", "eng", "English (Mauritius)", HunspellName: "en_MU ?"),
            //new LanguageInfo("en-MY", "eng", "English (Malaysia)", HunspellName: "en_MY ?"),
            //new LanguageInfo("en-NA", "eng", "English (Namibia)", HunspellName: "en_NA"),
            new LanguageInfo("en-NZ", "eng", "English (New Zealand)", HunspellName: "en_NZ"),
            //new LanguageInfo("en-PH", "eng", "English (Philippines)", HunspellName: "en_PH ?"),
            //new LanguageInfo("en-PK", "eng", "English (Pakistan)", HunspellName: "en_PK ?"),
            //new LanguageInfo("en-SG", "eng", "English (Singapore)", HunspellName: "en_SG ?"),
            //new LanguageInfo("en-TT", "eng", "English (Trinidad and Tobago)", HunspellName: "en_TT ?"),
            //new LanguageInfo("en-UM", "eng", "English (Minor Outlying Islands)", HunspellName: "en_UM ?"),
            new LanguageInfo("en-US", "eng", "English (United States)", HunspellName: "en_US"),
            //new LanguageInfo("en-VI", "eng", "English (Virgin Islands)", HunspellName: "en_VI ?"),
            new LanguageInfo("en-ZA", "eng", "English (South Africa)", HunspellName: "en_ZA"),
            //new LanguageInfo("en-ZW", "eng", "English (Zimbabwe)", HunspellName: "en_ZW ?"),
            new LanguageInfo("eo", "epo", "Esperanto", "eo_001", GoogleName: "eo", HunspellName: "eo-EO"),

            new LanguageInfo("es", "spa", "Spanish", "es_ES", GoogleName: "es"),
            new LanguageInfo("es-AR", "spa", "Spanish (Argentina)", HunspellName: "es_AR"),
            new LanguageInfo("es-BO", "spa", "Spanish (Bolivia)", HunspellName: "es_BO"),
            new LanguageInfo("es-CL", "spa", "Spanish (Chile)", HunspellName: "es_CL"),
            new LanguageInfo("es-CO", "spa", "Spanish (Colombia)", HunspellName: "es_CO"),
            new LanguageInfo("es-CR", "spa", "Spanish (Costa Rica)", HunspellName: "es_CR"),
            new LanguageInfo("es-DO", "spa", "Spanish (Dominican Republic)", HunspellName: "es_DO"),
            new LanguageInfo("es-EC", "spa", "Spanish (Ecuador)", HunspellName: "es_EC"),
            new LanguageInfo("es-ES", "spa", "Spanish (Spain)", HunspellName: "es_ES"),
            new LanguageInfo("es-GT", "spa", "Spanish (Guatemala)", HunspellName: "es_GT"),
            new LanguageInfo("es-HN", "spa", "Spanish (Honduras)", HunspellName: "es_HN"),
            new LanguageInfo("es-MX", "spa", "Spanish (Mexico)", HunspellName: "es_MX"),
            new LanguageInfo("es-NI", "spa", "Spanish (Nicaragua)", HunspellName: "es_NI"),
            new LanguageInfo("es-PA", "spa", "Spanish (Panama)", HunspellName: "es_PA"),
            new LanguageInfo("es-PE", "spa", "Spanish (Peru)", HunspellName: "es_PE"),
            new LanguageInfo("es-PR", "spa", "Spanish (Puerto Rico)", HunspellName: "es_PR"),
            new LanguageInfo("es-PY", "spa", "Spanish (Paraguay)", HunspellName: "es_PY"),
            new LanguageInfo("es-SV", "spa", "Spanish (El Salvador)", HunspellName: "es_SV"),
            new LanguageInfo("es-US", "spa", "Spanish (United States)", HunspellName: "es_US"),
            new LanguageInfo("es-UY", "spa", "Spanish (Uruguay)", HunspellName: "es_UY"),
            new LanguageInfo("es-VE", "spa", "Spanish (Venezuela)", HunspellName: "es_VE"),
            new LanguageInfo("et", "est", "Estonian", "et_EE", GoogleName: "et", HunspellName: "et-EE"),
            //new LanguageInfo("et-EE", "est", "Estonian (Estonia)", HunspellName: "et-EE"),
            new LanguageInfo("eu", "eus", "Basque", "eu_ES", GoogleName: "eu", HunspellName: "eu"),
            //new LanguageInfo("eu-ES", "eus", "Basque (Spain)", HunspellName: "eu"),
            new LanguageInfo("fa", "fas", "Persian", "fa_IR", GoogleName: "fa", HunspellName: "fa_IR"),
            new LanguageInfo("fi", "fin", "Finnish", "fi_FI", GoogleName: "fi"),
            new LanguageInfo("fi-FI", "fin", "Finnish (Finland)", HunspellName: "fi_FI"),
            new LanguageInfo("fil", "tgl", "Filipino", "tl_PH", GoogleName: "tl", HunspellName: "tl_PH"), // Tagalog
            new LanguageInfo("fj", "fij", "Fijian", "fj_FJ"),
            //new LanguageInfo("fj-FJ", "fij", "Fijian (Fiji)", HunspellName: "fj_FJ ?"),
            new LanguageInfo("fo", "fao", "Faroese", "fo_FO", HunspellName: "fo_FO"),
            //new LanguageInfo("fo-FO", "fao", "Faroese (Faroe Islands)", HunspellName: "fo_FO"),
            new LanguageInfo("fr", "fra", "French", "fr_FR", GoogleName: "fr", HunspellName: "fr_FR"),
            //new LanguageInfo("fr:classic", "fra", "French", "fr_FR", GoogleName: "fr", HunspellName: "fr-classique"),
            //new LanguageInfo("fr:reform 1990", "fra", "French", "fr_FR", GoogleName: "fr", HunspellName: "fr-reforme1990"),
            //new LanguageInfo("fr:classic+Reform", "fra", "French", "fr_FR", GoogleName: "fr", HunspellName: "fr-toutesvariantes"),
            //new LanguageInfo("fr", "fra", "French", "fr_FR", GoogleName: "fr"),
            //new LanguageInfo("fr-BE", "fra", "French (Belgium)", HunspellName: "fr_BE ?"),
            //new LanguageInfo("fr-BF", "fra", "French (Burkina Faso)", HunspellName: "fr_BF ?"),
            //new LanguageInfo("fr-BI", "fra", "French (Burundi)", HunspellName: "fr_BI ?"),
            //new LanguageInfo("fr-BJ", "fra", "French (Benin)", HunspellName: "fr_BJ ?"),
            //new LanguageInfo("fr-BL", "fra", "French (Saint Barthélemy)", HunspellName: "fr_BL ?"),
            //new LanguageInfo("fr-CA", "fra", "French (Canada)", HunspellName: "fr_CA ?"),
            //new LanguageInfo("fr-CD", "fra", "French (Congo - Kinshasa)", HunspellName: "fr_CD ?"),
            //new LanguageInfo("fr-CG", "fra", "French (Congo - Brazzaville)", HunspellName: "fr_CG ?"),
            //new LanguageInfo("fr-CF", "fra", "French (Central African Republic)", HunspellName: "fr_CF ?"),
            //new LanguageInfo("fr-CH", "fra", "French (Switzerland)", HunspellName: "fr_CH ?"),
            //new LanguageInfo("fr-CI", "fra", "French (Côte d’Ivoire)", HunspellName: "fr_CI ?"),
            //new LanguageInfo("fr-CM", "fra", "French (Cameroon)", HunspellName: "fr_CM ?"),
            //new LanguageInfo("fr-DJ", "fra", "French (Djibouti)", HunspellName: "fr_DJ ?"),
            //new LanguageInfo("fr-FR", "fra", "French (France)", HunspellName: "fr_FR"),
            //new LanguageInfo("fr-GA", "fra", "French (Gabon)", HunspellName: "fr_GA ?"),
            //new LanguageInfo("fr-GF", "fra", "French (French Guiana)", HunspellName: "fr_GF ?"),
            //new LanguageInfo("fr-GN", "fra", "French (Guinea)", HunspellName: "fr_GN ?"),
            //new LanguageInfo("fr-GP", "fra", "French (Guadeloupe)", HunspellName: "fr_GP ?"),
            //new LanguageInfo("fr-GQ", "fra", "French (Equatorial Guinea)", HunspellName: "fr_GQ ?"),
            //new LanguageInfo("fr-KM", "fra", "French (Comoros)", HunspellName: "fr_KM ?"),
            //new LanguageInfo("fr-LU", "fra", "French (Luxembourg)", HunspellName: "fr_LU ?"),
            //new LanguageInfo("fr-MC", "fra", "French (Monaco)", HunspellName: "fr_MC ?"),
            //new LanguageInfo("fr-MF", "fra", "French (Saint Martin)", HunspellName: "fr_MF ?"),
            //new LanguageInfo("fr-MG", "fra", "French (Madagascar)", HunspellName: "fr_MG ?"),
            //new LanguageInfo("fr-ML", "fra", "French (Mali)", HunspellName: "fr_ML ?"),
            //new LanguageInfo("fr-MQ", "fra", "French (Martinique)", HunspellName: "fr_MQ ?"),
            //new LanguageInfo("fr-NE", "fra", "French (Niger)", HunspellName: "fr_NE ?"),
            //new LanguageInfo("fr-RE", "fra", "French (Réunion)", HunspellName: "fr_RE ?"),
            //new LanguageInfo("fr-RW", "fra", "French (Rwanda)", HunspellName: "fr_RW ?"),
            //new LanguageInfo("fr-SN", "fra", "French (Senegal)", HunspellName: "fr_SN ?"),
            //new LanguageInfo("fr-TD", "fra", "French (Chad)", HunspellName: "fr_TD ?"),
            //new LanguageInfo("fr-TG", "fra", "French (Togo)", HunspellName: "fr_TG ?"),
            //new LanguageInfo("fr-YT", "fra", "French (Mayotte)", HunspellName: "fr_YT ?"),
            new LanguageInfo("fur", "fur", "Friulian", "fur_IT", HunspellName: "fur-IT"),
            //new LanguageInfo("fur-IT", "fur", "Friulian (Italy)", HunspellName: "fur-IT"),
            new LanguageInfo("fy", "fry", "Frisian", "fy_NL", HunspellName: "fy"),
            //new LanguageInfo("fy-NL", "fry", "Frisian (Netherlands)", HunspellName: "fy"),
            new LanguageInfo("ga", "gle", "Irish", "ga_IE", GoogleName: "ga", HunspellName: "ga"),
            //new LanguageInfo("ga-IE", "gle", "Irish (Ireland)", HunspellName: "ga"),
            new LanguageInfo("gd", "gla", "Scottish Gaelic", "gd_GB", HunspellName: "gd-GB"),
            //new LanguageInfo("gd-GB", "gla", "Scottish Gaelic (United Kingdom)", HunspellName: "gd-GB"),
            new LanguageInfo("gl", "glg", "Galician", "gl_ES", GoogleName: "gl", HunspellName: "gl_ES"),
            //new LanguageInfo("gl-ES", "glg", "Galician", HunspellName: "gl_ES"),
            new LanguageInfo("gsw", "gsw", "Alsatian", "gsw_FR"),
            //new LanguageInfo("gsw-FR", "gsw", "Alsatian (France)", HunspellName: "gsw_FR ?"),
            new LanguageInfo("gu", "guj", "Gujarati", "gu_IN", GoogleName: "gu"),
            //new LanguageInfo("gu-IN", "guj", "Gujarati (India)", HunspellName: "gu_IN ?"),
            new LanguageInfo("ha-Latn", "hau", "Hausa (Latin)", "ha-Latn_NG", GoogleName: "ha"),
            //new LanguageInfo("ha-Latn-NG", "hau", "Hausa (Latin, Nigeria)", HunspellName: "ha_Latn_NG ?"),
            new LanguageInfo("he", "heb", "Hebrew", "he_IL", GoogleName: "iw"),
            //new LanguageInfo("he-IL", "heb", "Hebrew (Israel)", HunspellName: "he_IL"),
            new LanguageInfo("hi", "hin", "Hindi", "hi_IN", GoogleName: "hi"),
            //new LanguageInfo("hi-IN", "hin", "Hindi (India)", HunspellName: "hi_IN ?"),
            new LanguageInfo("hmn", "hmn", "Hmong", "hmn_LA", GoogleName: "hmn", HunspellName: "hmn_LA"),
            new LanguageInfo("hr", "hrv", "Croatian", "hr_HR", GoogleName: "hr", HunspellName: "hr-HR"),
            //new LanguageInfo("hr-HR", "hrv", "Croatian (Croatia)", HunspellName: "hr-HR"),
            //new LanguageInfo("hr-BA", "hrb", "Croatian (Bosnia and Herzegovina)", HunspellName: "hr-BA ?"),
            new LanguageInfo("ht", "hat", "Haitian Creole", "ht_HT", GoogleName: "ht"),
            new LanguageInfo("hu", "hun", "Hungarian", "hu_HU", GoogleName: "hu", HunspellName: "hu_HU"),
            //new LanguageInfo("hu-HU", "hun", "Hungarian (Hungary)", HunspellName: "hu_HU"),
            new LanguageInfo("hy", "hye", "Armenian", "hy_AM", GoogleName: "hy", HunspellName: "hy_AM"),
            //new LanguageInfo("hy-AM", "hye", "Armenian (Armenia)", HunspellName: "hy_AM"),
            new LanguageInfo("ia", "ina", "Interlingua", "ia_001", HunspellName: "ia-ia"),
            new LanguageInfo("id", "ind", "Indonesian", "id_ID", GoogleName: "id"),
            //new LanguageInfo("id-ID", "ind", "Indonesian (Indonesia)", HunspellName: "id_ID ?"),
            new LanguageInfo("ig", "ibo", "Igbo", "ig_NG", GoogleName: "ig"),
            //new LanguageInfo("ig-NG", "ibo", "Igbo (Nigeria)", HunspellName: "ig_NG ?"),
            new LanguageInfo("ii", "iii", "Yi", "ii_CN"),
            //new LanguageInfo("ii-CN", "iii", "Yi (China)", HunspellName: "ii_CN ?"),
            new LanguageInfo("is", "isl", "Icelandic", "is_IS", GoogleName: "is", HunspellName: "is_IS"),
            //new LanguageInfo("is-IS", "isl", "Icelandic (Iceland)", HunspellName: "is_IS"),
            new LanguageInfo("it", "ita", "Italian", "it_IT", GoogleName: "it", HunspellName: "it_IT"),
            //new LanguageInfo("it-CH", "ita", "Italian (Switzerland)", HunspellName: "it_CH ?"),
            //new LanguageInfo("it-IT", "ita", "Italian (Italy)", HunspellName: "it_IT"),
            new LanguageInfo("iu-Cans", "iku", "Inuktitut (Syllabics)", "iu-Cans_CA"),
            //new LanguageInfo("iu-Cans-CA", "iku", "Inuktitut (Syllabics, Canada)", HunspellName: "iu_Cans_CA ?"),
            new LanguageInfo("iu-Latn", "iku", "Inuktitut (Latin)", "iu-Latn_CA"),
            //new LanguageInfo("iu-Latn-CA", "iku", "Inuktitut (Latin, Canada)", HunspellName: "iu_Latn_CA ?"),
            new LanguageInfo("ja", "jpn", "Japanese", "ja_JP", GoogleName: "ja"),
            //new LanguageInfo("ja-JP", "jpn", "Japanese (Japan)", HunspellName: "ja_JP ?"),
            new LanguageInfo("jv", "jav", "Javanese", "jv_ID", GoogleName: "jw", HunspellName: "jv_ID"),
            new LanguageInfo("ka", "kat", "Georgian", "ka_GE", GoogleName: "ka"),
            //new LanguageInfo("ka-GE", "kat", "Georgian (Georgia)", HunspellName: "ka_GE ?"),
            new LanguageInfo("kk", "kaz", "Kazakh", "kk_KZ", GoogleName: "kk"),
            //new LanguageInfo("kk-KZ", "kaz", "Kazakh (Kazakhstan)", HunspellName: "kk_KZ ?"),
            new LanguageInfo("kl", "kal", "Greenlandic", "kl_GL"),
            //new LanguageInfo("kl-GL", "kal", "Greenlandic (Greenland)", HunspellName: "kl_GL ?"),
            new LanguageInfo("km", "khm", "Khmer", "km_KH", GoogleName: "km"),
            //new LanguageInfo("km-KH", "khm", "Khmer (Cambodia)", HunspellName: "km_KH ?"),
            new LanguageInfo("kn", "kan", "Kannada", "kn_IN", GoogleName: "kn"),
            //new LanguageInfo("kn-IN", "kan", "Kannada (India)", HunspellName: "kn_IN ?"),
            new LanguageInfo("ko", "kor", "Korean", "ko_KR", GoogleName: "ko"),
            //new LanguageInfo("ko-KR", "kor", "Korean (Korea)", HunspellName: "ko_KR ?"),
            new LanguageInfo("kok", "kok", "Konkani", "kok_IN"),
            //new LanguageInfo("kok-IN", "kok", "Konkani (India)", HunspellName: "kok_IN ?"),
            new LanguageInfo("ky", "kir", "Kyrgyz", "ky_KG"),
            //new LanguageInfo("ky-KG", "kir", "Kyrgyz (Kyrgyzstan)", HunspellName: "ky_KG ?"),
            new LanguageInfo("la", "lat", "Latin", "la_VA", GoogleName: "la"),
            new LanguageInfo("lb", "ltz", "Luxembourgish", "lb_LU"),
            //new LanguageInfo("lb-LU", "ltz", "Luxembourgish (Luxembourg)", HunspellName: "lb_LU ?"),
            new LanguageInfo("lo", "lao", "Lao", "lo_LA", GoogleName: "lo"),
            new LanguageInfo("lo-LA", "lao", "Lao (Lao P.D.R.)", HunspellName: "lo_LA"),
            new LanguageInfo("lt", "lit", "Lithuanian", "lt_LT", GoogleName: "lt"),
            new LanguageInfo("lt-LT", "lit", "Lithuanian (Lithuania)", HunspellName: "lt_LT"),
            new LanguageInfo("lv", "lav", "Latvian", "lv_LV", GoogleName: "lv"),
            new LanguageInfo("lv-LV", "lav", "Latvian (Latvia)", HunspellName: "lv_LV"),
            new LanguageInfo("mg", "mlg", "Malagasy", "mg_MG", GoogleName: "mg"),
            new LanguageInfo("mi", "mri", "Maori", "mi_NZ", GoogleName: "mi"),
            new LanguageInfo("mi-NZ", "mri", "Maori (New Zealand)", HunspellName: "mi_NZ"),
            new LanguageInfo("mk", "mkd", "Macedonian", "mk_MK", GoogleName: "mk"),
            new LanguageInfo("mk-MK", "mkd", "Macedonian (FYROM)", HunspellName: "mk_MK"),
            new LanguageInfo("ml", "mym", "Malayalam", "ml_IN", GoogleName: "ml"),
            new LanguageInfo("ml-IN", "mym", "Malayalam (India)", HunspellName: "ml_IN"),
            new LanguageInfo("mn-Cyrl", "mon", "Mongolian (Cyrillic)", "mn_MN", GoogleName: "mn"),
            new LanguageInfo("mn-MN", "mon", "Mongolian (Cyrillic, Mongolia)", HunspellName: "mn_Cyrl_MN"),
            new LanguageInfo("mn-Mong", "mon", "Mongolian (Traditional)", "mn-Mong_CN"),
            new LanguageInfo("mn-Mong-CN", "mon", "Mongolian (Traditional, China)", HunspellName: "mn_Mong_CN"),
            new LanguageInfo("moh", "moh", "Mohawk", "moh_CA"),
            new LanguageInfo("moh-CA", "moh", "Mohawk (Canada)", HunspellName: "moh_CA"),
            new LanguageInfo("mr", "mar", "Marathi", "mr_IN", GoogleName: "mr"),
            new LanguageInfo("mr-IN", "mar", "Marathi (India)", HunspellName: "mr_IN"),
            new LanguageInfo("ms", "msa", "Malay", "ms_MY", GoogleName: "ms"),
            new LanguageInfo("ms-BN", "msa", "Malay (Brunei Darussalam)", HunspellName: "ms_BN"),
            new LanguageInfo("ms-MY", "msa", "Malay (Malaysia)", HunspellName: "ms_MY"),
            new LanguageInfo("mt", "mlt", "Maltese", "mt_MT", GoogleName: "mt"),
            new LanguageInfo("mt-MT", "mlt", "Maltese (Malta)", HunspellName: "mt_MT"),
            new LanguageInfo("my", "mya", "Burmese (Myanmar)", "my_MM", GoogleName: "my", HunspellName: "my_MM"),
            new LanguageInfo("ne", "nep", "Nepali", "ne_NP", GoogleName: "ne"),
            new LanguageInfo("ne-NP", "nep", "Nepali (Nepal)", HunspellName: "ne_NP"),
            new LanguageInfo("nl", "nld", "Dutch", "nl_NL", GoogleName: "nl", HunspellName: "nl"),
            //new LanguageInfo("nl-BE", "nld", "Dutch (Belgium)", HunspellName: "nl"),
            //new LanguageInfo("nl-NL", "nld", "Dutch (Netherlands)", HunspellName: "nl"),
            new LanguageInfo("no", "nob", "Norwegian", "nb_NO", GoogleName: "no"),
            new LanguageInfo("nb", "nob", "Norwegian (Bokmål)", "nb_NO"),
            new LanguageInfo("nb-NO", "nob", "Norwegian (Bokmål, Norway)", HunspellName: "nb_NO"),
            new LanguageInfo("nn", "nno", "Norwegian (Nynorsk)", "nn_NO"),
            new LanguageInfo("nn-NO", "nno", "Norwegian, (Nynorsk, Norway)", HunspellName: "nn_NO"),
            new LanguageInfo("nso", "nso", "Sesotho sa Leboa", "nso_ZA"),
            new LanguageInfo("nso-ZA", "nso", "Sesotho sa Leboa (South Africa)", HunspellName: "nso_ZA"),
            new LanguageInfo("ny", "nya", "Chichewa", "ny_MW", GoogleName: "ny", HunspellName: "ny_MW"),
            new LanguageInfo("oc", "oci", "Occitan", "oc_FR"),
            new LanguageInfo("oc-FR", "oci", "Occitan (France)", HunspellName: "oc_FR"),
            new LanguageInfo("or", "ori", "Oriya", "or_IN"),
            new LanguageInfo("or-IN", "ori", "Oriya (India)", HunspellName: "or_IN"),
            new LanguageInfo("pa", "pan", "Punjabi", "pa_IN", GoogleName: "pa"),
            new LanguageInfo("pa-IN", "pan", "Punjabi (India)", HunspellName: "pa_IN"),
            new LanguageInfo("pap-AW", "pap", "Papiamento (Aruba)", HunspellName: "Papiamento"),
            new LanguageInfo("pap-BQ", "pap", "Papiamentu (Bonaire)", HunspellName: "Papiamentu"),
            new LanguageInfo("pap-CW", "pap", "Papiamentu (Curaçao)", HunspellName: "Papiamentu"),
            new LanguageInfo("pl", "pol", "Polish", "pl_PL", GoogleName: "pl"),
            new LanguageInfo("pl-PL", "pol", "Polish (Poland)", HunspellName: "pl_PL"),
            new LanguageInfo("prs", "prs", "Dari", "prs_AF"),
            new LanguageInfo("prs-AF", "prs", "Dari (Afghanistan)", HunspellName: "prs_AF"),
            new LanguageInfo("ps", "pus", "Pashto", "ps_AF"),
            new LanguageInfo("ps-AF", "pus", "Pashto (Afghanistan)", HunspellName: "ps_AF"),
            new LanguageInfo("pt", "por", "Portuguese", "pt_PT", GoogleName: "pt"),
            new LanguageInfo("pt-BR", "por", "Portuguese (Brazil)", HunspellName: "pt_BR"),
            new LanguageInfo("pt-PT", "por", "Portuguese (Portugal)", HunspellName: "pt_PT"),
            new LanguageInfo("qut", "qut", "K'iche", "qut_GT"),
            //new LanguageInfo("qut-GT", "qut", "K'iche (Guatemala)", HunspellName: "qut_GT ?"),
            new LanguageInfo("quz", "qub", "Quechua", "quz_BO"),
            //new LanguageInfo("quz-BO", "qub", "Quechua (Bolivia)", HunspellName: "quz_BO ?"),
            //new LanguageInfo("quz-EC", "que", "Quechua (Ecuador)", HunspellName: "quz_EC ?"),
            //new LanguageInfo("quz-PE", "qup", "Quechua (Peru)", HunspellName: "quz_PE ?"),
            new LanguageInfo("rm", "roh", "Romansh", "rm_CH"),
            //new LanguageInfo("rm-CH", "roh", "Romansh (Switzerland)", HunspellName: "rm_CH ?"),
            new LanguageInfo("ro", "ron", "Romanian", "ro_RO", GoogleName: "ro"),
            new LanguageInfo("ro-RO", "ron", "Romanian (Romania)", HunspellName: "ro_RO"),
            new LanguageInfo("ru", "rus", "Russian", "ru_RU", GoogleName: "ru"),
            new LanguageInfo("ru-RU", "rus", "Russian (Russia)", HunspellName: "ru_RU"),
            new LanguageInfo("rw", "kin", "Kinyarwanda", "rw_RW"),
            new LanguageInfo("rw-RW", "kin", "Kinyarwanda (Rwanda)", HunspellName: "rw_RW"),
            new LanguageInfo("sa", "san", "Sanskrit", "sa_IN"),
            new LanguageInfo("sa-IN", "san", "Sanskrit (India)", HunspellName: "sa_IN"),
            new LanguageInfo("sah", "sah", "Yakut", "sah_RU"),
            new LanguageInfo("sah-RU", "sah", "Yakut (Russia)", HunspellName: "sah_RU"),
            new LanguageInfo("se", "sme", "Sami (Northern)", "se_NO"),
            new LanguageInfo("se-FI", "smg", "Sami (Northern, Finland)", HunspellName: "se_FI"),
            new LanguageInfo("se-NO", "sme", "Sami (Northern, Norway)", HunspellName: "se_NO"),
            new LanguageInfo("se-SE", "smf", "Sami (Northern, Sweden)", HunspellName: "se_SE"),
            new LanguageInfo("sma", "smb", "Sami (Southern)", "sma_SE"),
            new LanguageInfo("sma-NO", "sma", "Sami (Southern, Norway)", HunspellName: "sma_NO"),
            new LanguageInfo("sma-SE", "smb", "Sami (Southern, Sweden)", HunspellName: "sma_SE"),
            new LanguageInfo("smj", "smk", "Sami (Lule)", "smj_SE"),
            new LanguageInfo("smj-NO", "smj", "Sami (Lule, Norway)", HunspellName: "smj_NO"),
            new LanguageInfo("smj-SE", "smk", "Sami (Lule, Sweden)", HunspellName: "smj_SE"),
            new LanguageInfo("smn", "smn", "Sami (Inari)", "smn_FI"),
            new LanguageInfo("smn-FI", "smn", "Sami (Inari, Finland)", HunspellName: "smn_FI"),
            new LanguageInfo("sms", "sms", "Sami (Skolt)", "sms_FI"),
            new LanguageInfo("sms-FI", "sms", "Sami (Skolt, Finland)", HunspellName: "sms_FI"),
            new LanguageInfo("si", "sin", "Sinhala", "si_LK", GoogleName: "si"),
            new LanguageInfo("si-LK", "sin", "Sinhala (Sri Lanka)", HunspellName: "si_LK"),
            new LanguageInfo("sk", "slk", "Slovak", "sk_SK", GoogleName: "sk"),
            new LanguageInfo("sk-SK", "slk", "Slovak (Slovakia)", HunspellName: "sk_SK"),
            new LanguageInfo("sl", "slv", "Slovenian", "sl_SI", GoogleName: "sl"),
            new LanguageInfo("sl-SI", "slv", "Slovenian (Slovenia)", HunspellName: "sl_SI"),
            new LanguageInfo("so", "som", "Somali", "so_SO", GoogleName: "so"),
            new LanguageInfo("sq", "sqi", "Albanian", "sq_AL", GoogleName: "sq", HunspellName: "sq"),
            //new LanguageInfo("sq-AL", "sqi", "Albanian (Albania)", HunspellName: "sq_AL"),
            new LanguageInfo("sr-Cyrl", "srp", "Serbian (Cyrillic)", "sr-Cyrl_RS", HunspellName: "sr"),
            //new LanguageInfo("sr-Cyrl-BA", "srn", "Serbian (Cyrillic, Bosnia and Herzegovina)", HunspellName: "sr_Cyrl_BA ?"),
            //new LanguageInfo("sr-Cyrl-ME", "srp", "Serbian (Cyrillic, Montenegro)", HunspellName: "sr_Cyrl_ME ?"),
            //new LanguageInfo("sr-Cyrl-RS", "srp", "Serbian (Cyrillic, Serbia)", HunspellName: "sr_Cyrl_RS ?"),
            new LanguageInfo("sr-Latn", "srp", "Serbian (Latin)", "sr-Latn_RS", GoogleName: "sr", HunspellName: "sr-Latn"),
            //new LanguageInfo("sr-Latn-BA", "srs", "Serbian (Latin, Bosnia and Herzegovina)", HunspellName: "sr_Latn_BA ?"),
            //new LanguageInfo("sr-Latn-ME", "srp", "Serbian (Latin, Montenegro)", HunspellName: "sr_Latn_ME ?"),
            //new LanguageInfo("sr-Latn-RS", "srp", "Serbian (Latin, Serbia)", HunspellName: "sr_Latn_RS ?"),
            //new LanguageInfo("st", "sot", "Sesotho", "st_LS", GoogleName: "st", HunspellName: "st_LS"),
            new LanguageInfo("su", "sun", "Sundanese", "su_ID", GoogleName: "su", HunspellName: "su_ID"),
            new LanguageInfo("sv", "swe", "Swedish", "sv_SE", GoogleName: "sv", HunspellName: "sv_SE"),
            //new LanguageInfo("sv-FI", "swe", "Swedish (Finland)", HunspellName: "sv_FI ?"),
            //new LanguageInfo("sv-SE", "swe", "Swedish (Sweden)", HunspellName: "sv_SE"),
            new LanguageInfo("sw", "swa", "Kiswahili", "sw_KE", GoogleName: "sw"),
            new LanguageInfo("sw-KE", "swa", "Kiswahili (Kenya)", HunspellName: "sw_KE"),
            new LanguageInfo("syr", "syr", "Syriac", "syr_SY"),
            new LanguageInfo("syr-SY", "syr", "Syriac (Syria)", HunspellName: "syr_SY"),
            new LanguageInfo("ta", "tam", "Tamil", "ta_IN", GoogleName: "ta"),
            new LanguageInfo("ta-IN", "tam", "Tamil (India)", HunspellName: "ta_IN"),
            new LanguageInfo("te", "tel", "Telugu", "te_IN", GoogleName: "te"),
            new LanguageInfo("te-IN", "tel", "Telugu (India)", HunspellName: "te_IN"),
            new LanguageInfo("tg-Cyrl", "tgk", "Tajik (Cyrillic)", "tg-Cyrl_TJ", GoogleName: "tg"),
            new LanguageInfo("tg-Cyrl-TJ", "tgk", "Tajik (Cyrillic, Tajikistan)", HunspellName: "tg_Cyrl_TJ"),
            new LanguageInfo("th", "tha", "Thai", "th_TH", GoogleName: "th"),
            new LanguageInfo("th-TH", "tha", "Thai (Thailand)", HunspellName: "th_TH"),
            new LanguageInfo("ti", "tir", "Tigrinya", "ti_ER"),
            //new LanguageInfo("ti-ER", "tir", "Tigrinya (Eritrea)", HunspellName: "ti_ER ?"),
            //new LanguageInfo("ti-ET", "tir", "Tigrinya (Ethiopia)", HunspellName: "ti_ET ?"),
            new LanguageInfo("tk", "tuk", "Turkmen", "tk_TM"),
            //new LanguageInfo("tk-TM", "tuk", "Turkmen (Turkmenistan)", HunspellName: "tk_TM ?"),
            new LanguageInfo("tn", "tsn", "Setswana", "tn_ZA"),
            //new LanguageInfo("tn-ZA", "tsn", "Setswana (South Africa)", HunspellName: "tn_ZA ?"),
            new LanguageInfo("tr", "tur", "Turkish", "tr_TR", GoogleName: "tr", HunspellName: "tr_TR"),
            //new LanguageInfo("tr-TR", "tur", "Turkish (Turkey)", HunspellName: "tr_TR"),
            new LanguageInfo("tt", "tat", "Tatar", "tt_RU"),
            //new LanguageInfo("tt-RU", "tat", "Tatar (Russia)", HunspellName: "tt_RU ?"),
            new LanguageInfo("tzm-Latn", "tzm", "Tamazight (Latin)", "tzm-Latn_DZ"),
            //new LanguageInfo("tzm-Latn-DZ", "tzm", "Tamazight (Latin, Algeria)", HunspellName: "tzm_Latn_DZ ?"),
            new LanguageInfo("ug", "uig", "Uyghur", "ug_CN"),
            //new LanguageInfo("ug-CN", "uig", "Uyghur (China)", HunspellName: "ug_CN ?"),
            new LanguageInfo("uk", "ukr", "Ukrainian", "uk_UA", GoogleName: "uk", HunspellName: "uk_UA"),
            //new LanguageInfo("uk-UA", "ukr", "Ukrainian (Ukraine)", HunspellName: "uk_UA"),
            new LanguageInfo("ur", "urd", "Urdu", "ur_PK", GoogleName: "ur"),
            //new LanguageInfo("ur-PK", "urd", "Urdu (Pakistan)", HunspellName: "ur_PK ?"),
            new LanguageInfo("uz-Cyrl", "uzb", "Uzbek (Cyrillic)", "uz-Cyrl_UZ"),
            //new LanguageInfo("uz-Cyrl-UZ", "uzb", "Uzbek (Cyrillic, Uzbekistan)", HunspellName: "uz_Cyrl_UZ ?"),
            new LanguageInfo("uz-Latn", "uzb", "Uzbek (Latin)", "uz-Latn_UZ", GoogleName: "uz"),
            //new LanguageInfo("uz-Latn-UZ", "uzb", "Uzbek (Latin, Uzbekistan)", HunspellName: "uz_Latn_UZ ?"),
            new LanguageInfo("vi", "vie", "Vietnamese", "vi_VN", GoogleName: "vi"),
            //new LanguageInfo("vi-VN", "vie", "Vietnamese (Vietnam)", HunspellName: "vi_VN ?"),
            new LanguageInfo("wo", "wol", "Wolof", "wo_SN"),
            //new LanguageInfo("wo-SN", "wol", "Wolof (Senegal)", HunspellName: "wo_SN ?"),
            new LanguageInfo("xh", "xho", "isiXhosa", "xh_ZA"),
            //new LanguageInfo("xh-ZA", "xho", "isiXhosa (South Africa)", HunspellName: "xh_ZA ?"),
            new LanguageInfo("yi", "yid", "Yiddish", "yi_001", GoogleName: "yi", HunspellName: "yi"),
            new LanguageInfo("yo", "yor", "Yoruba", "yo_NG", GoogleName: "yo"),
            //new LanguageInfo("yo-NG", "yor", "Yoruba (Nigeria)", HunspellName: "yo_NG ?"),
            new LanguageInfo("zh-Hans", "zho", "Chinese (Simplified)", "zh_CN", GoogleName: "zh"),
            //new LanguageInfo("zh-CN", "zho", "Chinese (Simplified, China)", HunspellName: "zh_CN ?"),
            //new LanguageInfo("zh-SG", "zho", "Chinese (Simplified, Singapore)", HunspellName: "zh_SG ?"),
            new LanguageInfo("zh-Hant", "zho", "Chinese (Traditional)", "zh_TW", GoogleName: "zh-TW"),
            //new LanguageInfo("zh-HK", "zho", "Chinese (Traditional, Hong Kong)", HunspellName: "zh_HK ?"),
            //new LanguageInfo("zh-MO", "zho", "Chinese (Traditional, Macao)", HunspellName: "zh_MO ?"),
            //new LanguageInfo("zh-TW", "zho", "Chinese (Traditional, Taiwan)", HunspellName: "zh_TW ?"),
            new LanguageInfo("zu", "zul", "isiZulu", "zu_ZA", GoogleName: "zu"),
            //new LanguageInfo("zu-ZA", "zul", "isiZulu (South Africa)", HunspellName: "zu_ZA ?"),
        });
        private static readonly LanguagesByName SelectedLanguages = SupportedLanguages; // TODO: user selection in Settings

        #region Static Method Declarations

        private static readonly string[] AutoDetectWordsEnglish = { "we", "are", "and", "your?", "what" };
        private static readonly string[] AutoDetectWordsDanish = { "vi", "han", "og", "jeg", "var", "men", "gider", "bliver", "virkelig", "kommer", "tilbage", "Hej" };
        private static readonly string[] AutoDetectWordsNorwegian = { "vi", "er", "og", "jeg", "var", "men" };
        private static readonly string[] AutoDetectWordsSwedish = { "vi", "är", "och", "Jag", "inte", "för" };
        private static readonly string[] AutoDetectWordsSpanish = { "qué", "eso", "muy", "estoy?", "ahora", "hay", "tú", "así", "cuando", "cómo", "él", "sólo", "quiero", "gracias", "puedo", "bueno", "soy", "hacer", "fue", "eres", "usted", "tienes", "puede",
                                                                    "[Ss]eñor", "ese", "voy", "quién", "creo", "hola", "dónde", "sus", "verdad", "quieres", "mucho", "entonces", "estaba", "tiempo", "esa", "mejor", "hombre", "hace", "dios", "también", "están",
                                                                    "siempre", "hasta", "ahí", "siento", "puedes" };
        private static readonly string[] AutoDetectWordsItalian = { "Cosa", "sono", "Grazie", "Buongiorno", "bene", "questo", "ragazzi", "propriamente", "numero", "hanno", "giorno", "faccio", "davvero", "negativo", "essere", "vuole", "sensitivo", "venire" };
        private static readonly string[] AutoDetectWordsFrench = { "pas", "[vn]ous", "ça", "une", "pour", "[mt]oi", "dans", "elle", "tout", "plus", "[bmt]on", "suis", "avec", "oui", "fait", "ils", "être", "faire", "comme", "était", "quoi", "ici", "veux",
                                                                   "rien", "dit", "où", "votre", "pourquoi", "sont", "cette", "peux", "alors", "comment", "avez", "très", "même", "merci", "ont", "aussi", "chose", "voir", "allez", "tous", "ces", "deux" };
        private static readonly string[] AutoDetectWordsPortuguese = { "[Nn]ão", "[Ee]ntão", "uma", "ele", "bem", "isso", "você", "sim", "meu", "muito", "estou", "ela", "fazer", "tem", "já", "minha", "tudo", "só", "tenho", "agora", "vou", "seu", "quem",
                                                                       "há", "lhe", "quero", "nós", "coisa", "são", "ter", "dizer", "eles", "pode", "bom", "mesmo", "mim", "estava", "assim", "estão", "até", "quer", "temos", "acho", "obrigado", "também",
                                                                       "tens", "deus", "quê", "ainda", "noite" };
        private static readonly string[] AutoDetectWordsGerman = { "und", "auch", "sich", "bin", "hast", "möchte" };
        private static readonly string[] AutoDetectWordsDutch = { "van", "een", "[Hh]et", "m(ij|ĳ)", "z(ij|ĳ)n" };
        private static readonly string[] AutoDetectWordsPolish = { "Czy", "ale", "ty", "siê", "jest", "mnie" };
        private static readonly string[] AutoDetectWordsGreek = { "μου", "[Εε]ίναι", "αυτό", "Τόμπυ", "καλά", "Ενταξει", "πρεπει", "Λοιπον", "τιποτα", "ξερεις" };
        private static readonly string[] AutoDetectWordsRussian = { "[Ээч]?то", "[Нн]е", "[ТтМмбв]ы", "Да", "[Нн]ет", "Он", "его", "тебя", "как", "меня", "Но", "всё", "мне", "вас", "знаю", "ещё", "за", "нас", "чтобы", "был" };
        private static readonly string[] AutoDetectWordsUkrainian = { "[Нн]і", "[Пп]ривіт", "[Цц]е", "[Щщ]о", "[Йй]ого", "[Вв]ін", "[Яя]к", "[Гг]аразд", "[Яя]кщо", "[Мм]ені", "[Тт]вій", "[Її]х", "[Вв]ітаю", "[Дд]якую", "вже", "було", "був", "цього",
                                                                      "нічого", "немає", "може", "знову", "бо", "щось", "щоб", "цим", "тобі", "хотів", "твоїх", "мої", "мій", "має", "їм", "йому", "дуже" };
        private static readonly string[] AutoDetectWordsBulgarian = { "[Кк]акво", "тук", "може", "Как", "Ваше" };
        private static readonly string[] AutoDetectWordsArabic = { "من", "هل", "لا", "فى", "لقد", "ما" };
        private static readonly string[] AutoDetectWordsHebrew = { "אתה", "אולי", "הוא", "בסדר", "יודע", "טוב" };
        private static readonly string[] AutoDetectWordsVietnamese = { "không", "[Tt]ôi", "anh", "đó", "ông" };
        private static readonly string[] AutoDetectWordsHungarian = { "hogy", "lesz", "tudom", "vagy", "mondtam", "még" };
        private static readonly string[] AutoDetectWordsTurkish = { "için", "Tamam", "Hayır", "benim", "daha", "deðil", "önce", "lazým", "benim", "çalýþýyor", "burada", "efendim" };
        private static readonly string[] AutoDetectWordsCroatianAndSerbian = { "sam", "ali", "nije", "samo", "ovo", "kako", "dobro", "sve", "tako", "će", "mogu", "ću", "zašto", "nešto", "za" };
        private static readonly string[] AutoDetectWordsCroatian = { "što", "ovdje", "gdje", "kamo", "tko", "prije", "uvijek", "vrijeme", "vidjeti", "netko",
                                                                     "vidio", "nitko", "bok", "lijepo", "oprosti", "htio", "mjesto", "oprostite", "čovjek", "dolje",
                                                                     "čovječe", "dvije", "dijete", "dio", "poslije", "događa", "vjerovati", "vjerojatno", "vjerujem", "točno",
                                                                     "razumijem", "vidjela", "cijeli", "svijet", "obitelj", "volio", "sretan", "dovraga", "svijetu", "htjela",
                                                                     "vidjeli", "negdje", "želio", "ponovno", "djevojka", "umrijeti", "čovjeka", "mjesta", "djeca", "osjećam",
                                                                     "uopće", "djecu", "naprijed", "obitelji", "doista", "mjestu", "lijepa", "također", "riječ", "tijelo" };
        private static readonly string[] AutoDetectWordsSerbian = { "šta", "ovde", "gde", "ko", "pre", "uvek", "vreme", "videti", "neko",
                                                                    "video", "niko", "ćao", "lepo", "izvini", "hteo", "mesto", "izvinite", "čovek", "dole",
                                                                    "čoveče", "dve", "dete", "deo", "posle", "dešava", "verovati", "verovatno", "verujem", "tačno",
                                                                    "razumem", "videla", "ceo", "svet", "porodica", "voleo", "srećan", "dođavola", "svetu", "htela",
                                                                    "videli", "negde", "želeo", "ponovo", "devojka", "umreti", "čoveka", "mesta", "deca", "osećam",
                                                                    "uopšte", "decu", "napred", "porodicu", "zaista", "mestu", "lepa", "takođe", "reč", "telo" };
        private static readonly string[] AutoDetectWordsSerbianCyrillic = { "сам", "али", "није", "само", "ово", "како", "добро", "све", "тако", "ће", "могу", "ћу", "зашто", "нешто", "за", "шта", "овде" };
        private static readonly string[] AutoDetectWordsIndonesian = { "yang", "tahu", "bisa", "akan", "tahun", "tapi", "dengan", "untuk", "rumah", "dalam", "sudah", "bertemu" };
        private static readonly string[] AutoDetectWordsThai = { "โอ", "โรเบิร์ต", "วิตตอเรีย", "ดร", "คุณตำรวจ", "ราเชล", "ไม่", "เลดดิส", "พระเจ้า", "เท็ดดี้", "หัวหน้า", "แอนดรูว์" };
        private static readonly string[] AutoDetectWordsKorean = { "그리고", "아니야", "하지만", "말이야", "그들은", "우리가" };
        private static readonly string[] AutoDetectWordsFinnish = { "että", "kuin", "minä", "mitään", "Mutta", "siitä", "täällä", "poika", "Kiitos", "enää", "vielä", "tässä" };
        private static readonly string[] AutoDetectWordsRomanian1 = { "pentru", "oamenii", "decât", "[Vv]reau", "[Ss]înt", "Asteaptã", "Fãrã", "aici", "domnule", "trãiascã", "niciodatã", "înseamnã", "vorbesti", "fãcut", "spune" };
        private static readonly string[] AutoDetectWordsRomanian2 = { "pentru", "oamenii", "decat", "[Tt]rebuie", "[Aa]cum", "Poate", "vrea", "soare", "nevoie", "daca", "echilibrul", "vorbesti", "zeului", "atunci", "memoria", "soarele" };
        //
        // Add new languages to SupportedLanguages before adding them here!
        //
        private static string AutoDetectLanguage(string text, int bestCount)
        {
            int count = GetCount(text, AutoDetectWordsEnglish);
            if (count > bestCount)
            {
                int usCount = GetCount(text, "color", "flavor", "honor", "humor", "neighbor", "honor");
                int gbCount = GetCount(text, "colour", "flavour", "honour", "humour", "neighbour", "honour");
                return (gbCount > usCount) ? "en-GB" : "en-US";
            }

            count = GetCount(text, AutoDetectWordsDanish);
            if (count > bestCount)
            {
                int norwegianCount = GetCount(text, "ut", "deg", "meg", "merkelig", "mye", "spørre");
                int dutchCount = GetCount(text, AutoDetectWordsDutch);
                if (norwegianCount < 2 && dutchCount < count)
                    return "da";
            }

            count = GetCount(text, AutoDetectWordsNorwegian);
            if (count > bestCount)
            {
                int danishCount = GetCount(text, "siger", "dig", "mig", "mærkelig", "tilbage", "spørge");
                int dutchCount = GetCount(text, AutoDetectWordsDutch);
                if (danishCount < 2 && dutchCount < count)
                    return "no";
            }

            count = GetCount(text, AutoDetectWordsSwedish);
            if (count > bestCount)
                return "sv";

            count = GetCount(text, AutoDetectWordsSpanish);
            if (count > bestCount)
            {
                int frenchCount = GetCount(text, "[Cc]'est", "pas", "vous", "pour", "suis", "Pourquoi", "maison", "souviens", "quelque"); // not spanish words
                int portugueseCount = GetCount(text, "[NnCc]ão", "Então", "h?ouve", "pessoal", "rapariga", "tivesse", "fizeste",
                                                     "jantar", "conheço", "atenção", "foste", "milhões", "devias", "ganhar", "raios"); // not spanish words
                if (frenchCount < 2 && portugueseCount < 2)
                    return "es";
            }

            count = GetCount(text, AutoDetectWordsItalian);
            if (count > bestCount)
            {
                int frenchCount = GetCount(text, "[Cc]'est", "pas", "vous", "pour", "suis", "Pourquoi", "maison", "souviens", "quelque"); // not italian words
                if (frenchCount < 2)
                    return "it";
            }

            count = GetCount(text, AutoDetectWordsFrench);
            if (count > bestCount)
            {
                int romanianCount = GetCount(text, "[Vv]reau", "[Ss]înt", "[Aa]cum", "pentru", "domnule", "aici");
                if (romanianCount < 5)
                    return "fr";
            }

            count = GetCount(text, AutoDetectWordsPortuguese);
            if (count > bestCount)
                return "pt"; // Portuguese

            count = GetCount(text, AutoDetectWordsGerman);
            if (count > bestCount)
                return "de";

            count = GetCount(text, AutoDetectWordsDutch);
            if (count > bestCount)
                return "nl";

            count = GetCount(text, AutoDetectWordsPolish);
            if (count > bestCount)
                return "pl";

            count = GetCount(text, AutoDetectWordsGreek);
            if (count > bestCount)
                return "el"; // Greek

            count = GetCount(text, AutoDetectWordsRussian);
            if (count > bestCount)
                return "ru"; // Russian

            count = GetCount(text, AutoDetectWordsUkrainian);
            if (count > bestCount)
                return "uk"; // Ukrainian

            count = GetCount(text, AutoDetectWordsBulgarian);
            if (count > bestCount)
                return "bg"; // Bulgarian

            count = GetCount(text, AutoDetectWordsArabic);
            if (count > bestCount)
            {
                int hebrewCount = GetCount(text, AutoDetectWordsHebrew);
                if (hebrewCount < count)
                    return "ar"; // Arabic
            }

            count = GetCount(text, AutoDetectWordsHebrew);
            if (count > bestCount)
                return "he"; // Hebrew

            count = GetCount(text, AutoDetectWordsCroatianAndSerbian);
            if (count > bestCount)
            {
                int croatianCount = GetCount(text, AutoDetectWordsCroatian);
                int serbianCount = GetCount(text, AutoDetectWordsSerbian);
                if (croatianCount > serbianCount)
                    return "hr";  // Croatian
                return "sr-Latn"; // Serbian (Latin)
            }

            count = GetCount(text, AutoDetectWordsSerbianCyrillic);
            if (count > bestCount)
                return "sr-Cyrl"; // Serbian (Cyrillic)

            count = GetCount(text, AutoDetectWordsVietnamese);
            if (count > bestCount)
                return "vi"; // Vietnamese

            count = GetCount(text, AutoDetectWordsHungarian);
            if (count > bestCount)
                return "hu"; // Hungarian

            count = GetCount(text, AutoDetectWordsTurkish);
            if (count > bestCount)
                return "tr"; // Turkish

            count = GetCount(text, AutoDetectWordsIndonesian);
            if (count > bestCount)
                return "id"; // Indonesian

            count = GetCount(text, AutoDetectWordsThai);
            if (count > 10 || count > bestCount)
                return "th"; // Thai

            count = GetCount(text, AutoDetectWordsKorean);
            if (count > 10 || count > bestCount)
                return "ko"; // Korean

            count = GetCount(text, AutoDetectWordsFinnish);
            if (count > bestCount)
                return "fi"; // Finnish

            count = GetCount(text, AutoDetectWordsRomanian1);
            if (count <= bestCount)
                count = GetCount(text, AutoDetectWordsRomanian2);
            if (count > bestCount)
                return "ro"; // Romanian

            count = GetCountContains(text, "シ", "ュ", "シン", "シ", "ン", "ユ");
            count += GetCountContains(text, "イ", "ン", "チ", "ェ", "ク", "ハ");
            count += GetCountContains(text, "シ", "ュ", "う", "シ", "ン", "サ");
            count += GetCountContains(text, "シ", "ュ", "シ", "ン", "だ", "う");
            if (count > bestCount * 2)
                return "ja"; // Japanese - not tested...

            count = GetCountContains(text, "是", "是早", "吧", "的", "爱", "上好");
            count += GetCountContains(text, "的", "啊", "好", "好", "亲", "的");
            count += GetCountContains(text, "谢", "走", "吧", "晚", "上", "好");
            count += GetCountContains(text, "来", "卡", "拉", "吐", "滚", "他");
            if (count > bestCount * 2)
                return "zh-Hans"; // Chinese (Simplified) - not tested...

            return null;
        }

        public static LanguageInfo AutoDetectLanguageOrNull(Subtitle subtitle)
        {
            var sb = new StringBuilder();
            foreach (Paragraph p in subtitle.Paragraphs)
                sb.AppendLine(p.Text);

            var languageName = AutoDetectLanguage(sb.ToString(), subtitle.Paragraphs.Count / 14);
            if (languageName != null)
                return SupportedLanguages[languageName];
            return null;
        }

        public static LanguageInfo AutoDetectLanguage(Subtitle subtitle, Encoding encoding = null)
        {
            if (encoding != null)
            {
                switch (encoding.CodePage)
                {
                    case 860:
                        return SupportedLanguages["pt"]; // Portuguese
                    case 28599:
                    case 1254:
                        return SupportedLanguages["tr"]; // Turkish
                    case 28598:
                    case 1255:
                        return SupportedLanguages["he"]; // Hebrew
                    case 28596:
                    case 1256:
                        return SupportedLanguages["ar"]; // Arabic
                    case 1258:
                        return SupportedLanguages["vi"]; // Vietnamese
                    case 949:
                    case 1361:
                    case 20949:
                    case 51949:
                    case 50225:
                        return SupportedLanguages["ko"]; // Korean
                    case 1253:
                    case 28597:
                        return SupportedLanguages["el"]; // Greek
                    case 50220:
                    case 50221:
                    case 50222:
                    case 51932:
                    case 20932:
                    case 10001:
                        return SupportedLanguages["ja"]; // Japanese
                    case 20000:
                    case 20002:
                    case 20936:
                    case 950:
                    case 52936:
                    case 54936:
                    case 51936:
                        // TODO: is it possible to differentiate between zh-Hans and zh-Hant from encoding?
                        return SupportedLanguages["zh-Hans"]; // Chinese
                }
            }
            return AutoDetectLanguageOrNull(subtitle) ?? SupportedLanguages["en"];
        }

        public static LanguageInfo GetLanguageInfo(string languageName)
        {
            return SupportedLanguages.Contains(languageName) ? SupportedLanguages[languageName] : null;
        }

        private static int GetCount(string text, params string[] words)
        {
            int count = 0;
            for (int i = 0; i < words.Length; i++)
            {
                count += Regex.Matches(text, "\\b" + words[i] + "\\b", (RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture)).Count;
            }
            return count;
        }

        private static int GetCountContains(string text, params string[] words)
        {
            int count = 0;
            for (int i = 0; i < words.Length; i++)
            {
                var regEx = new Regex(words[i]);
                count += regEx.Matches(text).Count;
            }
            return count;
        }

        #endregion

        #region Static Property Declarations

        public static IEnumerable<LanguageInfo> AllLanguages
        {
            get
            {
                return SelectedLanguages;
            }
        }

        public static IEnumerable<LanguageInfo> NeutralLanguages // w/o territory locales
        {
            get
            {
                return SelectedLanguages.Where(li => li.IsNeutral);
            }
        }

        public static IEnumerable<LanguageInfo> GoogleLanguages
        {
            get
            {
                return SelectedLanguages.Where(li => li.GoogleName != null);
            }
        }

        public static IEnumerable<LanguageInfo> HunspellLanguages
        {
            get
            {
                return SelectedLanguages.Where(li => li.HunspellName != null);
            }
        }

        public static IEnumerable<LanguageInfo> TesseractLanguages
        {
            get
            {
                return SelectedLanguages.Where(li => li.TesseractName != null);
            }
        }

        #endregion

        private LanguageInfo(string name, string threeLetterIsoName, string englishName, string fullName = null, string GoogleName = null, string HunspellName = null, string TesseractName = null)
        {
            _name = name;
            _googleName = GoogleName;
            _hunspellName = HunspellName;
            _tesseractName = TesseractName;
            _threeLetterIsoName = threeLetterIsoName;

            var variant = string.Empty;
            var colonIndex = name.IndexOf(':', 2);
            if (colonIndex > 0 && name.Length - colonIndex > 1)
            {
                variant = name.Substring(colonIndex + 1).CapitalizeFirstLetter();
                name = name.Remove(colonIndex);
            }
            try
            {
                _culture = CultureInfo.GetCultureInfo(name);
                _displayName = _culture.DisplayName;
            }
            catch
            {
                _culture = CultureInfo.InvariantCulture;
                _displayName = englishName;
                try
                {
                    switch (name)
                    {
                        case "zh-Hans":
                            _culture = CultureInfo.GetCultureInfo(0x0004); // WindowsXP fallback
                            _displayName = _culture.DisplayName;
                            break;
                        case "zh-Hant":
                            _culture = CultureInfo.GetCultureInfo(0x7C04); // WindowsXP fallback
                            _displayName = _culture.DisplayName;
                            break;
                    }
                }
                catch
                {
                }
            }
            if (variant.Length > 0)
            {
                var index = _displayName.LastIndexOf(')');
                if (index > 0)
                {
                    _displayName = _displayName.Insert(index, ", " + variant);
                }
                else
                {
                    _displayName += " (" + variant + ")";
                }
            }

            _isNeutral = true;
            if (fullName == null)
            {
                _isNeutral = false;
                var index = name.LastIndexOf('-');
                fullName = name.Remove(index, 1).Insert(index, "_");
            }
            _fullName = fullName;
        }

        public int CompareTo(LanguageInfo li)
        {
            int cmp = 1;
            if (!object.ReferenceEquals(li, null))
            {
                cmp = string.Compare(DisplayName, li.DisplayName, StringComparison.CurrentCultureIgnoreCase);
                if (cmp == 0)
                {
                    cmp = string.Compare(Name, li.Name, StringComparison.OrdinalIgnoreCase);
                }
            }
            return cmp;
        }

        public bool Equals(LanguageInfo li)
        {
            return object.ReferenceEquals(li, null) ? false : Name.Equals(li.Name, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as LanguageInfo);
        }

        public override int GetHashCode()
        {
            return Name.ToUpperInvariant().GetHashCode();
        }

        public override string ToString()
        {
            return DisplayName;
        }

        public static bool operator == (LanguageInfo li1, LanguageInfo li2)
        {
            return object.ReferenceEquals(li2, null) ? object.ReferenceEquals(li1, null) : li2.Equals(li1);
        }

        public static bool operator != (LanguageInfo li1, LanguageInfo li2)
        {
            return object.ReferenceEquals(li2, null) ? !object.ReferenceEquals(li1, null) : !li2.Equals(li1);
        }

        #region Property Declarations

        public LanguageInfo GoogleInfo
        {
            get
            {
                var li = this;
                if (GoogleName == null)
                {
                    li = NeutralInfo;
                    if (li.GoogleName == null)
                    {
                        li = SupportedLanguages["en"];
                    }
                }
                return li;
            }
        }

        public LanguageInfo NeutralInfo
        {
            get
            {
                return IsNeutral ? this : SupportedLanguages[FullName.Remove(FullName.LastIndexOf('_'))];
            }
        }

        private readonly string _name;
        /// <summary>
        /// Unicode (Windows) name: «language»[-«script»][-«region»]
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }
        private readonly string _fullName;
        /// <summary>
        /// Unicode name with underscore separator preceding region: «language»[-«script»]_«region»
        /// </summary>
        public string FullName
        {
            get
            {
                return _fullName;
            }
        }
        private readonly string _displayName;
        /// <summary>
        /// Descriptive name in the CurrentUI language or in English if n/a
        /// </summary>
        public string DisplayName
        {
            get
            {
                return _displayName;
            }
        }
        private readonly string _threeLetterIsoName;
        /// <summary>
        /// ISO 639-2 language code
        /// </summary>
        public string ThreeLetterIsoName
        {
            get
            {
                return _threeLetterIsoName;
            }
        }
        private readonly CultureInfo _culture;
        /// <summary>
        /// .NET CultureInfo for this language or CultureInfo.InvariantCulture if n/a
        /// </summary>
        public CultureInfo Culture
        {
            get
            {
                return _culture;
            }
        }
        private readonly bool _isNeutral;
        /// <summary>
        /// Language or territory locale? (neutral/specific)
        /// </summary>
        public bool IsNeutral
        {
            get
            {
                return _isNeutral;
            }
        }
        private readonly string _googleName;
        /// <summary>
        /// Name used by Google Translate API
        /// </summary>
        public string GoogleName
        {
            get
            {
                return _googleName;
            }
        }
        private readonly string _hunspellName;
        /// <summary>
        /// Name of Hunspell dictionary w/o extension
        /// </summary>
        public string HunspellName
        {
            get
            {
                return _hunspellName;
            }
        }
        private readonly string _tesseractName;
        /// <summary>
        /// Name used by Tesseract
        /// </summary>
        public string TesseractName
        {
            get
            {
                return _tesseractName;
            }
        }

        #endregion

        private class LanguagesByName : KeyedCollection<string, LanguageInfo>
        {
            public LanguagesByName(ICollection<LanguageInfo> languages)
            {
                foreach (var language in languages.OrderBy(li => li.DisplayName, StringComparer.CurrentCultureIgnoreCase).ThenBy(li => li.Name, StringComparer.Ordinal))
                {
                    Add(language);
                }
            }

            protected override string GetKeyForItem(LanguageInfo li)
            {
                return li.Name;
            }
        }

    }
}