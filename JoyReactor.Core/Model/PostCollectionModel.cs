﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using JoyReactor.Core.Model.DTO;
using JoyReactor.Core.Model.Parser;
using JoyReactor.Core.Model.Parser.Data;
using JoyReactor.Core.Model.Database;
using Ninject;
using JoyReactor.Core.Model.Inject;

namespace JoyReactor.Core.Model
{
	public class PostCollectionModel : IPostCollectionModel
	{
//		private IEnumerable<ISiteParser> parsers = InjectService.Instance.GetAll<ISiteParser>();
//		private ISiteParser[] parsers = new ISiteParser[] { InjectService.Instance.Get<ISiteParser>() };
		private ISiteParser[] parsers = new ISiteParser[] { new ReactorParser() };


		#region IPostCollectionModel implementation

		public Task<List<Post>> GetPostsAsync (ID id, SyncFlags flags = SyncFlags.None)
		{
			return Task.Run(
				() => {
					if (flags == SyncFlags.First) SyncFirstPage(id);
					else if (flags == SyncFlags.Next) SyncNextPage(id);

					var sid = ToFlatId(id);
					return MainDb.Instance.Query<Post>("SELECT * FROM posts WHERE Id IN (SELECT PostId FROM tag_post WHERE TagId IN (SELECT Id FROM tags WHERE TagId = ?))", sid);
				});
		}

		public int GetCount (ID id)
		{
			return MainDb.Instance.ExecuteScalar<int> (
				"SELECT COUNT(*) FROM tag_post WHERE TagId IN (SELECT Id FROM tags WHERE TagId = ?)", 
				ToFlatId(id));
		}

		#endregion

		private string ToFlatId (ID id)
		{
			return id.Site + "-" + id.Type + "-" + id.Tag;
		}

		private void SyncFirstPage (ID id)
		{
			var p = parsers.First (s => s.ParserId == id.Site);

			p.ExtractTagPostCollection (id.Type, id.Tag, 0, state => {
				if (state.State == CollectionExportState.ExportState.Begin) {
					MainDb.Instance.Execute(
						"DELETE FROM tag_post WHERE TagId IN (SELECT Id FROM tags WHERE TagId = ?)",
						ToFlatId(id));
				} else if (state.State == CollectionExportState.ExportState.PostItem) {
					SavePostToDatabase(id, state.PostItem);
				}
			});
		}

		private void SavePostToDatabase (ID listId, ExportPost post)
		{
			var p = Convert (listId.Site, post);
			var f = ToFlatId (listId);

			p.Id = MainDb.Instance.ExecuteScalar<int> ("SELECT Id FROM posts WHERE PostId = ?", p.PostId);
			if (p.Id == 0) MainDb.Instance.Insert (p);
			else MainDb.Instance.Update (p);

			var tp = new TagPost ();
			tp.PostId = p.Id;
			tp.TagId = MainDb.Instance.ExecuteScalar<int> ("SELECT Id FROM tags WHERE TagId = ?", f);
			tp.Status = TagPost.StatusNew;
			MainDb.Instance.Insert (tp);
		}

		private void SyncNextPage (ID id)
		{
			throw new NotImplementedException ();
		}

		private Post Convert(ID.SiteParser parserId, ExportPost p)
		{
			return new Post {
				PostId = parserId + "-" + p.id,
				CommentCount = p.commentCount,
				Coub = p.coub,
				Created = p.created,
				Image = p.image,
				ImageHeight = p.imageHeight,
				ImageWidth = p.imageWidth,
				Rating = p.rating,
				Title = p.title,
				UserImage = p.userImage,
				UserName = p.userName,
			};
		}
	}
}