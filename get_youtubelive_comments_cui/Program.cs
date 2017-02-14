using System;
using System.Collections.Generic;
using System.Text;
using Mono.Options;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using Codeplex.Data;
using System.Threading;

namespace get_youtubelive_comments_cui
{
	class Program
	{
		static void Main(string[] args)
		{
			string channel_id = null;
			string video_id =null;
			string api_key = null;
			string live_chat_id = null;

			bool show_help = false;
			bool show_owners_message = false;

			int interval = 1000;

			bool send_to_bouyomi = false;
			string[] bouyomi_origin = null;
			string bouyomi_host = "localhost";
			int bouyomi_port = 50001;
			string bouyomi_prefix = null;

			dynamic messages_object = null;
			var messages_dictionary = new Dictionary<string, object[]>();
			var messages_ids = new List<string>();
			var messages_ids_old = new List<string>();
			var messages_ids_diff = new List<string>();

			var option = new OptionSet()
			{
				{"h", "ヘルプを表示する", v => show_help = v != null},
				{"o", "チャンネルオーナーのメッセージを表示する", v => show_owners_message = v != null},
				{"i=", "取得間隔を指定する", v => interval = (int)(float.Parse(v) * 1000)},
				{"b:", "棒読みちゃんに送信する", v => {send_to_bouyomi = v != null; bouyomi_origin = v.Split('=');}},
				{"p=", "棒読みちゃんに送信するプレフィックスを指定する", v => bouyomi_prefix = v}
			};

			var other_args = option.Parse(args);

			if (other_args.Count < 2)
			{
				show_usage();

				return;
			}

			switch (other_args[0].Length)
			{
				case 24:
					channel_id = other_args[0];
					break;

				case 11:
					video_id = other_args[0];
					break;

				default:
					show_usage();

					return;
			}

			api_key = other_args[1];

			if (send_to_bouyomi == true)
			{
				if (bouyomi_origin[0] != "")
				{
					bouyomi_host = bouyomi_origin[0];
				}

				if (bouyomi_origin[1] != "")
				{
					bouyomi_port = Int32.Parse(bouyomi_origin[1]);
				}
			}

			if (video_id == null)
			{
				var video_id_request = WebRequest.Create("https://www.youtube.com/channel/" + channel_id + "/videos?flow=list&live_view=501&view=2");

				try
				{
					using (var video_id_response = video_id_request.GetResponse())
					{
						using (var video_id_stream = new StreamReader(video_id_response.GetResponseStream(), Encoding.UTF8))
						{
							var video_id_regex = new Regex("href=\"\\/watch\\?v=(.+?)\"", RegexOptions.IgnoreCase | RegexOptions.Singleline);

							var video_id_match = video_id_regex.Match(video_id_stream.ReadToEnd());

							if (!video_id_match.Success)
							{
								Console.Error.WriteLine("Error: ストリーミングが見つかりませんでした");

								return;
							}

							var index1 = video_id_match.Value.LastIndexOf('=') + 1;
							var index2 = video_id_match.Value.LastIndexOf('"');

							video_id = video_id_match.Value.Substring(index1, index2 - index1);
						}
					}
				}
				catch
				{
					Console.Error.WriteLine("Error: ストリーミングの検索に失敗しました");

					return;
				}
			}			

			var live_chat_id_request = WebRequest.Create("https://www.googleapis.com/youtube/v3/videos?part=liveStreamingDetails&id=" + video_id + "&key=" + api_key);

			try
			{
				using (var live_chat_id_response = live_chat_id_request.GetResponse())
				{
					using (var live_chat_id_stream = new StreamReader(live_chat_id_response.GetResponseStream(), Encoding.UTF8))
					{
						var live_chat_id_object = DynamicJson.Parse(live_chat_id_stream.ReadToEnd());

						live_chat_id = live_chat_id_object.items[0].liveStreamingDetails.activeLiveChatId;

						if (live_chat_id == null)
						{
							Console.Error.WriteLine("Error: Live Chat IDの取得に失敗しました");

							return;
						}
					}
				}
			}
			catch
			{
				Console.Error.WriteLine("Error: Live Chat IDの取得に失敗しました");

				return;
			}

			while (true)
			{
				var messages_request = WebRequest.Create("https://www.googleapis.com/youtube/v3/liveChat/messages?part=snippet,authorDetails&liveChatId=" + live_chat_id + "&key=" + api_key);

				messages_object = null;

				using (var messages_response = messages_request.GetResponse())
				{
					using (var messages_stream = new StreamReader(messages_response.GetResponseStream()))
					{
						messages_object = DynamicJson.Parse(messages_stream.ReadToEnd());
					}
				}

				messages_ids.Clear();
				messages_dictionary.Clear();

				foreach (var value in messages_object.items)
				{
					messages_ids.Add(value.id);

					messages_dictionary.Add(value.id, new object[]
					{
						value.authorDetails.displayName,
						value.snippet.textMessageDetails.messageText,
						value.authorDetails.isChatOwner
					});
				}

				messages_ids_diff = new List<string>(messages_ids);
				messages_ids_diff.RemoveAll(messages_ids_old.Contains);

				foreach (var value in messages_ids_diff)
				{
					if (show_owners_message || !Convert.ToBoolean(messages_dictionary[value][2]))
					{
						Console.WriteLine(messages_dictionary[value][0] + " : " + messages_dictionary[value][1]);
					}
				}

				messages_ids_old.Clear();
				messages_ids_old = new List<string>(messages_ids);

				messages_ids_diff.Clear();

				Thread.Sleep(interval);
			}

		}

		static void show_usage()
		{
			Console.Error.WriteLine("使用法1: get_youtubelive_comments_cui video_id api_key [option]...");
			Console.Error.WriteLine("使用法2: get_youtubelive_comments_cui channel_id api_key [option]...");
			Console.Error.WriteLine("'get_youtubelive_comments_cui -h' でヘルプを表示します");
		}
	}
}
