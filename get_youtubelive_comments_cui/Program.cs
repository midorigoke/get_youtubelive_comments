using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Options;

namespace get_youtubelive_comments_cui
{
	class Program
	{
		static void Main(string[] args)
		{
			string channel_id = null;
			string video_id =null;
			string api_key = null;

			bool show_help = false;
			bool show_owners_message = false;

			bool send_to_bouyomi = false;
			string[] bouyomi_origin = null;
			string bouyomi_host = "localhost";
			int bouyomi_port = 50001;
			string bouyomi_prefix = null;

			List<string> other_args;

			var option = new OptionSet()
			{
				{"h", "ヘルプを表示する", v => show_help = v != null},
				{"o", "チャンネルオーナーのメッセージを表示する", v => show_owners_message = v != null},
				{"b:", "棒読みちゃんに送信する", v => {send_to_bouyomi = v != null; bouyomi_origin = v.Split('=');}},
				{"p=", "棒読みちゃんに送信するプレフィックスを指定する", v => bouyomi_prefix = v}
			};

			other_args = option.Parse(args);

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
		}

		static void show_usage()
		{
			Console.Error.WriteLine("使用法1: get_youtubelive_comments_cui video_id api_key [option]...");
			Console.Error.WriteLine("使用法2: get_youtubelive_comments_cui channel_id api_key [option]...");
			Console.Error.WriteLine("'get_youtubelive_comments_cui -h' でヘルプを表示します");
		}
	}
}
