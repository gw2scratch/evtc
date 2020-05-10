using System;
using Eto.Forms;

namespace GW2Scratch.ArcdpsLogManager.Commands
{
	public class About : Command
	{
		public About()
		{
			MenuText = "About";
			Shortcut = Keys.F11;
		}

		protected override void OnExecuted(EventArgs e)
		{
			base.OnExecuted(e);

			string license = @"
Summary

- Software: MIT Licence
- Profession and specialization images: Guild Wars 2 Wiki contributors, GNU FDL v1.2 or newer
- Other images: ArenaNet, All Rights Reserved

Profession and specialization images

	Permission is granted to copy, distribute and/or modify this document
	under the terms of the GNU Free Documentation License, Version 1.3
	or any later version published by the Free Software Foundation;
	with no Invariant Sections, no Front-Cover Texts, and no Back-Cover Texts.
	A copy of the license is included in the section entitled ""GNU
	Free Documentation License"".

	For more details, see https://www.gnu.org/licenses/fdl-1.3.html.

Non-profession images

	©2010–2018 ArenaNet, LLC. All rights reserved. Guild Wars, Guild Wars 2,
	Heart of Thorns, Guild Wars 2: Path of Fire, ArenaNet, NCSOFT, the Interlocking NC Logo,
	and all associated logos and designs are trademarks or registered trademarks of NCSOFT Corporation.
	All other trademarks are the property of their respective owners.

	The Terms of Use are further detailed on the following URL:
	https://www.guildwars2.com/en/legal/guild-wars-2-content-terms-of-use/

The software

	MIT License

	Copyright (c) 2018

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the ""Software""), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in all
	copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
	SOFTWARE.";

			var about = new AboutDialog
			{
				Logo = Resources.GetProgramIcon(),
				Website = new Uri("https://discord.gg/TnHpN34"),
				WebsiteLabel = "Discord Server",
				License = license
			};
			about.ShowDialog(Application.Instance.MainForm);
		}
	}
}