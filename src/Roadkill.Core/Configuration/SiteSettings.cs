﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Roadkill.Core.Logging;

namespace Roadkill.Core.Configuration
{
	/// <summary>
	/// Contains all configuration data stored with NHibernate/the database, for settings that do not require an application restart when changed.
	/// </summary>
	[Serializable]
	public class SiteSettings
	{
		internal static readonly Guid SiteSettingsId = new Guid("b960e8e5-529f-4f7c-aee4-28eb23e13dbd");

		private string _allowedFileTypes;

		/// <summary>
		/// The files types allowed for uploading.
		/// </summary>
		public string AllowedFileTypes
		{
			get
			{
				if (string.IsNullOrEmpty(_allowedFileTypes))
				{
					Log.Warn("The allowed file types setting is empty - populating with default types jpg, png, gif.");
					_allowedFileTypes = "jpg, png, gif";
				}

				return _allowedFileTypes;
			}
			set
			{
				_allowedFileTypes = value;
			}
		}

		/// <summary>
		/// Whether users can register themselves, or if the administrators should do it. 
		/// If windows authentication is enabled, this setting is ignored.
		/// </summary>
		public bool AllowUserSignup { get; set; }

		/// <summary>
		/// Whether to Recaptcha is enabled for user signups and password resets.
		/// </summary>
		public bool IsRecaptchaEnabled { get; set; }

		/// <summary>
		/// The type of markup used: Three available options are: Creole, Markdown, MediaWiki.
		/// The default is Creole.
		/// </summary>
		/// <remarks>This is a string because it's easier with the Javascript interaction.</remarks>
		public string MarkupType { get; set; }

		/// <summary>
		/// The private key for the recaptcha service, if enabled. This is optained when you sign up for the free service at https://www.google.com/recaptcha/.
		/// </summary>
		public string RecaptchaPrivateKey { get; set; }

		/// <summary>
		/// The public key for the recaptcha service, if enabled. This is optained when you sign up for the free service at https://www.google.com/recaptcha/.
		/// </summary>
		public string RecaptchaPublicKey { get; set; }

		/// <summary>
		/// The full url of the site.
		/// </summary>
		public string SiteUrl { get; set; }

		/// <summary>
		/// The title of the site.
		/// </summary>
		public string SiteName { get; set; }

		/// <summary>
		/// The site theme, defaults to "Blackbar"
		/// </summary>
		public string Theme { get; set; }

		/// <summary>
		/// An asp.net relativate path e.g. ~/Themes/ to the current theme directory. Does not include a trailing slash.
		/// </summary>
		[JsonIgnore]
		public string ThemePath
		{
			get
			{
				return string.Format("~/Themes/{0}", Theme);
			}
		}

		/// <summary>
		/// Retrieves a list of the file extensions that are permitted for upload.
		/// </summary>
		[JsonIgnore]
		public List<string> AllowedFileTypesList
		{
			get
			{
				return new List<string>(AllowedFileTypes.Replace(" ", "").Split(','));
			}
		}

		public SiteSettings()
		{
			AllowedFileTypes = "jpg, png, gif";
			AllowUserSignup = false;
			IsRecaptchaEnabled = false;
			Theme = "Mediawiki";
			MarkupType = "Creole";
			SiteName = "Your site";
			SiteUrl = "";
			RecaptchaPrivateKey = "";
			RecaptchaPublicKey = "";
		}

		public string GetJson()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}

		public static SiteSettings LoadFromJson(string json)
		{
			if (string.IsNullOrEmpty(json))
			{
				Log.Warn("SitePreferences.LoadFromJson - json string was empty (returning a default SitePreferences object)");
				return new SiteSettings();
			}

			try
			{
				return JsonConvert.DeserializeObject<SiteSettings>(json);
			}
			catch (JsonReaderException e)
			{
				Log.Error("SitePreferences.LoadFromJson - an exception occurred deserializing the JSON - {0}", e.ToString());
				return new SiteSettings();
			}
		}
	}
}