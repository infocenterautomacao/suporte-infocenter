using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;

[assembly: AssemblyTitle("Suporte Infocenter")]
[assembly: AssemblyCopyright("Copyright © Infocenter Automação 2026")]
[assembly: CompilationRelaxations(8)]
[assembly: AssemblyProduct("Suporte Infocenter")]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: AssemblyFileVersion("1.2.0.0")]
[assembly: AssemblyVersion("1.2.0.0")]
namespace AtualizadorGUI
{
	internal static class Cores
	{
		public static readonly Color NavyEscuro = Color.FromArgb(32, 30, 31);

		public static readonly Color NavyMedio = Color.FromArgb(1, 150, 218);

		public static readonly Color AzulClaro = Color.FromArgb(41, 175, 235);

		public static readonly Color Laranja = Color.FromArgb(255, 140, 0);

		public static readonly Color Branco = Color.White;

		public static readonly Color FundoApp = Color.FromArgb(32, 30, 31);

		public static readonly Color FundoCard = Color.FromArgb(45, 42, 43);

		public static readonly Color TextoPrincipal = Color.White;

		public static readonly Color TextoSub = Color.FromArgb(200, 205, 215);

		public static readonly Color FundoLog = Color.FromArgb(26, 24, 25);

		public static readonly Color TextoLog = Color.White;

		public static readonly Color Verde = Color.FromArgb(46, 204, 113);

		public static readonly Color Vermelho = Color.FromArgb(231, 76, 60);

		public static readonly Color Amarelo = Color.FromArgb(241, 196, 15);

		public static readonly Color BarraStatus = Color.FromArgb(32, 30, 31);
	}
	internal class BotaoRounded : Button
	{
		private bool _hovered;

		public int Radius { get; set; }

		public Color HoverColor { get; set; }

		public Color NormalColor { get; set; }

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams createParams = base.CreateParams;
				createParams.ExStyle |= 32;
				return createParams;
			}
		}

		public BotaoRounded()
		{
			Radius = 8;
			base.FlatStyle = FlatStyle.Flat;
			base.FlatAppearance.BorderSize = 0;
			Cursor = Cursors.Hand;
			Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
			ForeColor = Color.White;
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);
			_hovered = true;
			Invalidate();
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			_hovered = false;
			Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			Color color = ((_hovered && HoverColor != Color.Empty) ? HoverColor : ((NormalColor != Color.Empty) ? NormalColor : BackColor));
			using (SolidBrush brush = new SolidBrush(color))
			{
				using (GraphicsPath path = GetRoundedPath(base.ClientRectangle, Radius))
				{
					e.Graphics.FillPath(brush, path);
				}
			}
			if (!_hovered)
			{
				using (Pen pen = new Pen(Color.FromArgb(30, 0, 0, 0), 2f))
				{
					Rectangle rect = new Rectangle(2, 2, base.Width - 3, base.Height - 3);
					using (GraphicsPath path2 = GetRoundedPath(rect, Radius))
					{
						e.Graphics.DrawPath(pen, path2);
					}
				}
			}
			TextRenderer.DrawText(e.Graphics, Text, Font, base.ClientRectangle, ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
		}

		private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			int num = radius * 2;
			graphicsPath.AddArc(rect.X, rect.Y, num, num, 180f, 90f);
			graphicsPath.AddArc(rect.Right - num, rect.Y, num, num, 270f, 90f);
			graphicsPath.AddArc(rect.Right - num, rect.Bottom - num, num, num, 0f, 90f);
			graphicsPath.AddArc(rect.X, rect.Bottom - num, num, num, 90f, 90f);
			graphicsPath.CloseFigure();
			return graphicsPath;
		}
	}
	internal class CardPainel : Panel
	{
		public int Radius { get; set; }

		public Color CustomFundoColor { get; set; }

		public CardPainel()
		{
			Radius = 14;
			CustomFundoColor = Color.Empty;
			DoubleBuffered = true;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			Color color = ((CustomFundoColor != Color.Empty) ? CustomFundoColor : Cores.FundoCard);
			using (SolidBrush brush = new SolidBrush(color))
			{
				using (GraphicsPath path = GetRoundedPath(new Rectangle(1, 1, base.Width - 3, base.Height - 3), Radius))
				{
					e.Graphics.FillPath(brush, path);
				}
			}
			using (GraphicsPath path2 = GetRoundedPath(new Rectangle(1, 1, base.Width - 3, base.Height - 3), Radius))
			{
				using (Pen pen = new Pen(Cores.NavyMedio, 1.5f))
				{
					e.Graphics.DrawPath(pen, path2);
				}
			}
		}

		private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			int num = radius * 2;
			graphicsPath.AddArc(rect.X, rect.Y, num, num, 180f, 90f);
			graphicsPath.AddArc(rect.Right - num, rect.Y, num, num, 270f, 90f);
			graphicsPath.AddArc(rect.Right - num, rect.Bottom - num, num, num, 0f, 90f);
			graphicsPath.AddArc(rect.X, rect.Bottom - num, num, num, 90f, 90f);
			graphicsPath.CloseFigure();
			return graphicsPath;
		}
	}
	internal class FormPrincipal : Form
	{
		private const string URL_VERSAO = "https://raw.githubusercontent.com/infocenterautomacao/suporte-infocenter/main/versao.txt";
		private const string URL_EXE = "https://raw.githubusercontent.com/infocenterautomacao/suporte-infocenter/main/SuporteInfocenter.exe";
		private const string VERSAO_ATUAL = "1.2.0.0";
		private BotaoRounded btnUpdateApp;

		private const string BASE_UP = "https://www.codigoup.com/painel/files/UpSystem%20v";

		private const string BASE_FORCA = "https://www.codigoup.com/painel/files/ServidorForcaVendas%20v";

		private const string CONFIG_FILE = "config.txt";

		private Panel pnlHeader;

		private Label lblEmpresa;

		private Label lblSubtitulo;

		private CardPainel cardUp;

		private CardPainel cardForca;

		private CardPainel cardLog;

		private Label lblUpTitulo;

		private Label lblUpVersao;

		private Label lblUpStatus;

		private Label lblUpPasta;

		private Label lblForcaTitulo;

		private Label lblForcaVersao;

		private Label lblForcaStatus;

		private Label lblForcaPasta;

		private BotaoRounded btnUp;

		private BotaoRounded btnForca;

		private Button btnUpAlterar;

		private Button btnForcaAlterar;

		private ProgressBar pbUp;

		private ProgressBar pbForca;

		private RichTextBox rtbLog;

		private Label lblStatusBar;

		private Label lblVersaoApp;

		private Button btnFechar;

		private Button btnMinimizar;

		private bool _dragging;

		private Point _dragStart;

		private string pathUp = "C:\\UpSystem";

		private string pathForca = "C:\\UpSystem\\ForcaVendas";

		public FormPrincipal()
		{
			InitUI();
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			ServicePointManager.DefaultConnectionLimit = 50;
			EventHandler value = delegate
			{
				InicializarDeteccao();
				ChecarAtualizacaoSuporte();
			};
			base.Shown += value;
		}

		private Image CarregarLogo()
		{
			try
			{
				Assembly executingAssembly = Assembly.GetExecutingAssembly();
				using (Stream stream = executingAssembly.GetManifestResourceStream("logo.png"))
				{
					if (stream != null)
					{
						return Image.FromStream(stream);
					}
				}
			}
			catch
			{
			}
			return null;
		}

		private Icon CarregarIcone()
		{
			try
			{
				Assembly executingAssembly = Assembly.GetExecutingAssembly();
				using (Stream stream = executingAssembly.GetManifestResourceStream("icone.png"))
				{
					if (stream != null)
					{
						using (Bitmap bitmap = new Bitmap(stream))
						{
							IntPtr hicon = bitmap.GetHicon();
							return Icon.FromHandle(hicon);
						}
					}
				}
			}
			catch
			{
			}
			return null;
		}

		private void CarregarConfiguracao()
		{
			try
			{
				string directoryName = Path.GetDirectoryName(Application.ExecutablePath);
				string path = Path.Combine(directoryName, "config.txt");
				if (!File.Exists(path))
				{
					return;
				}
				string[] array = File.ReadAllLines(path);
				string[] array2 = array;
				foreach (string text in array2)
				{
					string[] array3 = text.Split(new char[1] { '=' }, 2);
					if (array3.Length == 2)
					{
						string text2 = array3[0].Trim();
						string text3 = array3[1].Trim();
						if (text2 == "UpSystemPath")
						{
							pathUp = text3;
						}
						else if (text2 == "ForcaVendasPath")
						{
							pathForca = text3;
						}
					}
				}
			}
			catch
			{
			}
		}

		private void SalvarConfiguracao()
		{
			try
			{
				string directoryName = Path.GetDirectoryName(Application.ExecutablePath);
				string path = Path.Combine(directoryName, "config.txt");
				File.WriteAllLines(path, new string[2]
				{
					"UpSystemPath=" + pathUp,
					"ForcaVendasPath=" + pathForca
				});
			}
			catch
			{
			}
		}

		private void EscolherPasta(string tipo)
		{
			using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
			{
				folderBrowserDialog.Description = "Selecione a pasta de instalação do " + ((tipo == "UpSystem") ? "UpSystem" : "Servidor Força de Vendas");
				folderBrowserDialog.SelectedPath = ((tipo == "UpSystem") ? pathUp : pathForca);
				if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
				{
					if (tipo == "UpSystem")
					{
						pathUp = folderBrowserDialog.SelectedPath;
						lblUpPasta.Text = "Pasta: " + pathUp;
					}
					else
					{
						pathForca = folderBrowserDialog.SelectedPath;
						lblForcaPasta.Text = "Pasta: " + pathForca;
					}
					SalvarConfiguracao();
					Log(string.Format("[{0}] Pasta alterada para: {1}", tipo, folderBrowserDialog.SelectedPath), Cores.AzulClaro);
					DetectarVersaoProduto(tipo);
				}
			}
		}

		private void DetectarVersaoProduto(string tipo)
		{
			Task.Run(delegate
			{
				string maj;
				string min;
				string pat;
				bool ok = TryDetectLocalVersion(tipo, out maj, out min, out pat);
				Invoke((Action)delegate
				{
					if (tipo == "UpSystem")
					{
						string arg = (ok ? string.Format("{0}-{1}-{2}", maj, min, pat) : "Não detectada");
						lblUpVersao.Text = (ok ? string.Format("Versão local: {0}", arg) : "Nenhum arquivo local encontrado");
						lblUpStatus.Text = (ok ? "✔ Local OK" : "⚠ Não encontrado");
						lblUpStatus.ForeColor = (ok ? Cores.Verde : Cores.Vermelho);
						lblUpPasta.Text = "Pasta: " + pathUp;
					}
					else
					{
						string arg2 = (ok ? string.Format("{0}-{1}", maj, pat) : "Não detectada");
						lblForcaVersao.Text = (ok ? string.Format("Versão local: {0}", arg2) : "Nenhum arquivo local encontrado");
						lblForcaStatus.Text = (ok ? "✔ Local OK" : "⚠ Não encontrado");
						lblForcaStatus.ForeColor = (ok ? Cores.Verde : Cores.Vermelho);
						lblForcaPasta.Text = "Pasta: " + pathForca;
					}
				});
			});
		}

		private void InitUI()
		{
			Text = "Suporte Infocenter - Infocenter Automação";
			base.Size = new Size(680, 640);
			base.FormBorderStyle = FormBorderStyle.None;
			base.StartPosition = FormStartPosition.CenterScreen;
			BackColor = Cores.FundoApp;
			DoubleBuffered = true;
			Icon icon = CarregarIcone();
			if (icon != null)
			{
				base.Icon = icon;
			}
			SetWindowRegion();
			pnlHeader = new Panel
			{
				Dock = DockStyle.Top,
				Height = 80,
				BackColor = Cores.NavyEscuro
			};
			pnlHeader.MouseDown += PnlHeader_MouseDown;
			pnlHeader.MouseMove += PnlHeader_MouseMove;
			pnlHeader.MouseUp += delegate
			{
				_dragging = false;
			};
			lblEmpresa = new Label
			{
				Text = "ATUALIZAÇÃO DE SISTEMA",
				Font = new Font("Segoe UI", 13f, FontStyle.Bold),
				ForeColor = Color.White,
				AutoSize = true
			};
			lblSubtitulo = new Label
			{
				Text = "Infocenter Automação",
				Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
				ForeColor = Color.FromArgb(1, 150, 218),
				AutoSize = true
			};
			btnMinimizar = CriarBotaoSistema("—", 14f);
			btnMinimizar.Location = new Point(base.Width - 78, 0);
			btnMinimizar.Size = new Size(38, 36);
			btnMinimizar.Click += delegate
			{
				base.WindowState = FormWindowState.Minimized;
			};
			btnFechar = CriarBotaoSistema("✕", 13f);
			btnFechar.Location = new Point(base.Width - 40, 0);
			btnFechar.Size = new Size(38, 36);
			btnFechar.BackColor = Color.FromArgb(200, 60, 50);
			btnFechar.FlatAppearance.MouseOverBackColor = Color.FromArgb(220, 80, 70);
			btnFechar.Click += delegate
			{
				Application.Exit();
			};
			Image image = CarregarLogo();
			if (image != null)
			{
				PictureBox pictureBox = new PictureBox();
				pictureBox.Image = image;
				pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
				pictureBox.Location = new Point(15, 10);
				pictureBox.Size = new Size(170, 60);
				pictureBox.BackColor = Color.Transparent;
				PictureBox pictureBox2 = pictureBox;
				pictureBox2.MouseDown += PnlHeader_MouseDown;
				pictureBox2.MouseMove += PnlHeader_MouseMove;
				pictureBox2.MouseUp += delegate
				{
					_dragging = false;
				};
				pnlHeader.Controls.Add(pictureBox2);
				lblEmpresa.Location = new Point(200, 18);
				lblSubtitulo.Location = new Point(202, 44);
			}
			else
			{
				lblEmpresa.Location = new Point(18, 18);
				lblSubtitulo.Location = new Point(20, 44);
			}
			Panel panel = new Panel();
			panel.Dock = DockStyle.Bottom;
			panel.Height = 2;
			panel.BackColor = Cores.NavyMedio;
			Panel value = panel;
			pnlHeader.Controls.Add(value);
			pnlHeader.Controls.AddRange(new Control[4] { lblEmpresa, lblSubtitulo, btnMinimizar, btnFechar });
			
			btnUpdateApp = new BotaoRounded
			{
				Text = "NOVA VERSÃO DISPONÍVEL! CLIQUE PARA ATUALIZAR O SUPORTE INFOCENTER.",
				Dock = DockStyle.Top,
				Height = 40,
				NormalColor = Cores.Laranja,
				HoverColor = Color.Red,
				BackColor = Cores.FundoApp,
				ForeColor = Color.White,
				Font = new Font("Segoe UI", 11f, FontStyle.Bold),
				Visible = false,
				Cursor = Cursors.Hand
			};
			btnUpdateApp.Click += BtnUpdateApp_Click;

			base.Controls.Add(btnUpdateApp);
			base.Controls.Add(pnlHeader);
			lblStatusBar = new Label
			{
				Text = "Pronto.",
				Font = new Font("Segoe UI", 9f),
				ForeColor = Color.FromArgb(160, 200, 255),
				Location = new Point(22, 600),
				Size = new Size(450, 25),
				TextAlign = ContentAlignment.MiddleLeft,
				BackColor = Color.Transparent
			};
			lblVersaoApp = new Label
			{
				Text = "Versão 1.2",
				Font = new Font("Segoe UI", 9f, FontStyle.Bold),
				ForeColor = Color.FromArgb(120, 130, 140),
				Location = new Point(558, 600),
				Size = new Size(100, 25),
				TextAlign = ContentAlignment.MiddleRight,
				BackColor = Color.Transparent
			};
			base.Controls.AddRange(new Control[2] { lblStatusBar, lblVersaoApp });
			cardUp = CriarCard(new Point(20, 95), new Size(310, 255));
			cardForca = CriarCard(new Point(350, 95), new Size(310, 255));
			ConstruirCardUp();
			ConstruirCardForca();
			base.Controls.AddRange(new Control[2] { cardUp, cardForca });
			cardLog = CriarCard(new Point(20, 370), new Size(640, 210));
			cardLog.CustomFundoColor = Cores.FundoLog;
			rtbLog = new RichTextBox
			{
				Location = new Point(10, 10),
				Size = new Size(620, 190),
				BackColor = Cores.FundoLog,
				ForeColor = Cores.TextoLog,
				Font = new Font("Consolas", 9.5f),
				ReadOnly = true,
				BorderStyle = BorderStyle.None,
				ScrollBars = RichTextBoxScrollBars.Vertical
			};
			cardLog.Controls.Add(rtbLog);
			base.Controls.Add(cardLog);
			Log("Sistema iniciado. Detectando versões locais...", Cores.TextoLog);
		}

		private CardPainel CriarCard(Point loc, Size sz)
		{
			CardPainel cardPainel = new CardPainel();
			cardPainel.Location = loc;
			cardPainel.Size = sz;
			cardPainel.BackColor = Cores.FundoApp;
			return cardPainel;
		}

		private void ConstruirCardUp()
		{
			Label label = new Label();
			label.AutoSize = false;
			label.Size = new Size(70, 60);
			label.Location = new Point(20, 20);
			label.BackColor = Color.Transparent;
			Label label2 = label;
			label2.Paint += delegate(object s, PaintEventArgs ev)
			{
				ev.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
				using (Pen pen = new Pen(Color.White, 3f))
				{
					ev.Graphics.DrawRectangle(pen, 15, 12, 40, 26);
					Point[] points = new Point[4]
					{
						new Point(6, 42),
						new Point(64, 42),
						new Point(58, 48),
						new Point(12, 48)
					};
					ev.Graphics.DrawPolygon(pen, points);
				}
			};
			lblUpTitulo = new Label
			{
				Text = "UpSystem",
				Font = new Font("Segoe UI", 15f, FontStyle.Bold),
				ForeColor = Color.White,
				AutoSize = true,
				Location = new Point(95, 22),
				BackColor = Color.Transparent
			};
			lblUpVersao = new Label
			{
				Text = "Detectando versão...",
				Font = new Font("Segoe UI", 8.5f),
				ForeColor = Cores.TextoSub,
				AutoSize = true,
				Location = new Point(95, 48),
				BackColor = Color.Transparent
			};
			lblUpStatus = new Label
			{
				Text = "",
				Font = new Font("Segoe UI", 8f, FontStyle.Bold),
				ForeColor = Cores.Amarelo,
				AutoSize = true,
				Location = new Point(95, 68),
				BackColor = Color.Transparent
			};
			lblUpPasta = new Label
			{
				Text = "Pasta: " + pathUp,
				Font = new Font("Segoe UI", 8f),
				ForeColor = Cores.TextoSub,
				AutoSize = false,
				Location = new Point(20, 105),
				Size = new Size(270, 32),
				AutoEllipsis = true,
				BackColor = Color.Transparent
			};
			btnUpAlterar = new Button
			{
				Text = "\ud83d\udcc1 Alterar Pasta",
				Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
				Location = new Point(20, 140),
				Size = new Size(110, 24),
				FlatStyle = FlatStyle.Flat,
				BackColor = Color.FromArgb(62, 60, 61),
				ForeColor = Color.White,
				Cursor = Cursors.Hand
			};
			btnUpAlterar.FlatAppearance.BorderSize = 0;
			btnUpAlterar.Click += delegate
			{
				EscolherPasta("UpSystem");
			};
			pbUp = new ProgressBar
			{
				Location = new Point(20, 180),
				Size = new Size(270, 10),
				Style = ProgressBarStyle.Continuous,
				Visible = false,
				Minimum = 0,
				Maximum = 100
			};
			btnUp = new BotaoRounded
			{
				Text = "Verificar Atualizações",
				Location = new Point(20, 205),
				Size = new Size(270, 40),
				NormalColor = Cores.NavyMedio,
				HoverColor = Cores.AzulClaro,
				BackColor = Cores.NavyMedio
			};
			btnUp.Click += async delegate
			{
				await ExecutarAtualizacao("UpSystem");
			};
			cardUp.Controls.AddRange(new Control[8] { label2, lblUpTitulo, lblUpVersao, lblUpStatus, lblUpPasta, btnUpAlterar, pbUp, btnUp });
		}

		private void ConstruirCardForca()
		{
			Label label = new Label();
			label.AutoSize = false;
			label.Size = new Size(70, 60);
			label.Location = new Point(20, 20);
			label.BackColor = Color.Transparent;
			Label label2 = label;
			label2.Paint += delegate(object s, PaintEventArgs ev)
			{
				ev.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
				using (Pen pen = new Pen(Color.White, 3f))
				{
					ev.Graphics.DrawRectangle(pen, 12, 10, 46, 32);
					ev.Graphics.DrawLine(pen, 35, 42, 35, 50);
					ev.Graphics.DrawLine(pen, 23, 50, 47, 50);
				}
			};
			lblForcaTitulo = new Label
			{
				Text = "Força de Vendas",
				Font = new Font("Segoe UI", 13f, FontStyle.Bold),
				ForeColor = Color.White,
				AutoSize = true,
				Location = new Point(95, 22),
				BackColor = Color.Transparent
			};
			lblForcaVersao = new Label
			{
				Text = "Detectando versão...",
				Font = new Font("Segoe UI", 8.5f),
				ForeColor = Cores.TextoSub,
				AutoSize = true,
				Location = new Point(95, 48),
				BackColor = Color.Transparent
			};
			lblForcaStatus = new Label
			{
				Text = "",
				Font = new Font("Segoe UI", 8f, FontStyle.Bold),
				ForeColor = Cores.Amarelo,
				AutoSize = true,
				Location = new Point(95, 68),
				BackColor = Color.Transparent
			};
			lblForcaPasta = new Label
			{
				Text = "Pasta: " + pathForca,
				Font = new Font("Segoe UI", 8f),
				ForeColor = Cores.TextoSub,
				AutoSize = false,
				Location = new Point(20, 105),
				Size = new Size(270, 32),
				AutoEllipsis = true,
				BackColor = Color.Transparent
			};
			btnForcaAlterar = new Button
			{
				Text = "\ud83d\udcc1 Alterar Pasta",
				Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
				Location = new Point(20, 140),
				Size = new Size(110, 24),
				FlatStyle = FlatStyle.Flat,
				BackColor = Color.FromArgb(62, 60, 61),
				ForeColor = Color.White,
				Cursor = Cursors.Hand
			};
			btnForcaAlterar.FlatAppearance.BorderSize = 0;
			btnForcaAlterar.Click += delegate
			{
				EscolherPasta("ServidorForcaVendas");
			};
			pbForca = new ProgressBar
			{
				Location = new Point(20, 180),
				Size = new Size(270, 10),
				Style = ProgressBarStyle.Continuous,
				Visible = false,
				Minimum = 0,
				Maximum = 100
			};
			btnForca = new BotaoRounded
			{
				Text = "Verificar Atualizações",
				Location = new Point(20, 205),
				Size = new Size(270, 40),
				NormalColor = Cores.NavyMedio,
				HoverColor = Cores.AzulClaro,
				BackColor = Cores.NavyMedio
			};
			btnForca.Click += async delegate
			{
				await ExecutarAtualizacao("ServidorForcaVendas");
			};
			cardForca.Controls.AddRange(new Control[8] { label2, lblForcaTitulo, lblForcaVersao, lblForcaStatus, lblForcaPasta, btnForcaAlterar, pbForca, btnForca });
		}

		private Button CriarBotaoSistema(string texto, float fontSize)
		{
			Button button = new Button();
			button.Text = texto;
			button.Font = new Font("Segoe UI", fontSize);
			button.FlatStyle = FlatStyle.Flat;
			button.ForeColor = Color.White;
			button.BackColor = Color.Transparent;
			button.Cursor = Cursors.Hand;
			Button button2 = button;
			button2.FlatAppearance.BorderSize = 0;
			button2.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 255, 255, 255);
			return button2;
		}

		private void SetWindowRegion()
		{
			int num = 10;
			GraphicsPath graphicsPath = new GraphicsPath();
			Rectangle rectangle = new Rectangle(0, 0, base.Width, base.Height);
			graphicsPath.AddArc(rectangle.X, rectangle.Y, num * 2, num * 2, 180f, 90f);
			graphicsPath.AddArc(rectangle.Right - num * 2, rectangle.Y, num * 2, num * 2, 270f, 90f);
			graphicsPath.AddArc(rectangle.Right - num * 2, rectangle.Bottom - num * 2, num * 2, num * 2, 0f, 90f);
			graphicsPath.AddArc(rectangle.X, rectangle.Bottom - num * 2, num * 2, num * 2, 90f, 90f);
			graphicsPath.CloseFigure();
			base.Region = new Region(graphicsPath);
		}

		private void PnlHeader_MouseDown(object s, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				_dragging = true;
				_dragStart = e.Location;
			}
		}

		private void PnlHeader_MouseMove(object s, MouseEventArgs e)
		{
			if (_dragging)
			{
				base.Location = new Point(base.Left + e.X - _dragStart.X, base.Top + e.Y - _dragStart.Y);
			}
		}

		private void InicializarDeteccao()
		{
			CarregarConfiguracao();
			lblUpPasta.Text = "Pasta: " + pathUp;
			lblForcaPasta.Text = "Pasta: " + pathForca;
			DetectarVersaoProduto("UpSystem");
			DetectarVersaoProduto("ServidorForcaVendas");
			Log("Detecção de versões locais concluída.", Cores.Verde);
			SetStatus("Pronto.");
		}

		private async Task ExecutarAtualizacao(string tipo)
		{
			bool isUp = tipo == "UpSystem";
			BotaoRounded btnAtual = (isUp ? btnUp : btnForca);
			Label lblStatus = (isUp ? lblUpStatus : lblForcaStatus);
			Label lblVersao = (isUp ? lblUpVersao : lblForcaVersao);
			ProgressBar pb = (isUp ? pbUp : pbForca);
			string baseUrl = (isUp ? "https://www.codigoup.com/painel/files/UpSystem%20v" : "https://www.codigoup.com/painel/files/ServidorForcaVendas%20v");
			btnAtual.Enabled = false;
			pb.Visible = true;
			pb.Value = 0;
			string major;
			string minor;
			string startPatch;
			bool ok = TryDetectLocalVersion(tipo, out major, out minor, out startPatch);
			int patchNum = 0;
			if (!ok)
			{
				lblStatus.ForeColor = Cores.Amarelo;
				lblStatus.Text = "⚠ Versão não detectada localmente";
				Log(string.Format("[{0}] Não foi possível detectar versão local. Informe manualmente.", tipo), Cores.Amarelo);
				string text = Interaction.InputBox(isUp ? "Digite a versão no formato  Major-Minor-Patch  (ex: 1-13-3):" : "Digite a versão no formato  Major-Patch  (ex: 2-16):", "Versão não detectada");
				if (string.IsNullOrWhiteSpace(text))
				{
					btnAtual.Enabled = true;
					pb.Visible = false;
					return;
				}
				string[] array = text.Trim().Split('-');
				if (isUp && array.Length == 3)
				{
					major = array[0];
					minor = array[1];
					startPatch = array[2];
				}
				else
				{
					if (isUp || array.Length != 2)
					{
						MessageBox.Show("Formato inválido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						btnAtual.Enabled = true;
						pb.Visible = false;
						return;
					}
					major = array[0];
					startPatch = array[1];
				}
			}
			patchNum = int.Parse(startPatch);
			SetStatus(string.Format("Buscando versões disponíveis para {0}...", tipo));
			Log(string.Format("[{0}] Iniciando busca paralela a partir do patch {1} (testando +30)...", tipo, patchNum), Cores.AzulClaro);
			List<string> disponiveis = null;
			await Task.Run(async delegate
			{
				disponiveis = await ScanVersionsAsync(baseUrl, major, minor, patchNum, 30, isUp);
			});
			if (disponiveis.Count == 0)
			{
				lblStatus.ForeColor = Cores.Verde;
				lblStatus.Text = "✔ Sistema atualizado!";
				Log(string.Format("[{0}] Nenhuma atualização encontrada. Sistema está na versão mais recente.", tipo), Cores.Verde);
				SetStatus("Nenhuma atualização disponível.");
				MessageBox.Show(string.Format("O {0} está na versão mais recente!\nNenhuma atualização encontrada no servidor.", tipo), "Tudo atualizado!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				btnAtual.Enabled = true;
				pb.Visible = false;
				return;
			}
			string ultima = disponiveis.Last();
			Log(string.Format("[{0}] Versões disponíveis: {1}", tipo, string.Join(", ", disponiveis.ToArray())), Cores.Laranja);
			string escolha = ultima;
			if (disponiveis.Count > 1)
			{
				using (FormSelecionarVersao formSelecionarVersao = new FormSelecionarVersao(disponiveis, tipo))
				{
					if (formSelecionarVersao.ShowDialog() != DialogResult.OK)
					{
						btnAtual.Enabled = true;
						pb.Visible = false;
						SetStatus("Download cancelado.");
						return;
					}
					escolha = formSelecionarVersao.VersaoEscolhida;
				}
			}
			else
			{
				DialogResult dialogResult = MessageBox.Show(string.Format("Nova versão disponível: {0}\n\nDeseja baixar agora?", escolha), "Atualização disponível", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if (dialogResult == DialogResult.No)
				{
					btnAtual.Enabled = true;
					pb.Visible = false;
					SetStatus("Download cancelado.");
					return;
				}
			}
			string targetDir = (isUp ? pathUp : pathForca);
			if (!Directory.Exists(targetDir))
			{
				try
				{
					Directory.CreateDirectory(targetDir);
				}
				catch
				{
				}
			}
			string nomeArq = (isUp ? string.Format("UpSystem v{0}.rar", escolha) : string.Format("ServidorForcaVendas v{0}.rar", escolha));
			string destino = Path.Combine(targetDir, nomeArq);
			string urlFinal = baseUrl + escolha + ".rar";
			lblStatus.ForeColor = Cores.AzulClaro;
			lblStatus.Text = "⬇ Baixando...";
			SetStatus(string.Format("Baixando {0}...", nomeArq));
			Log(string.Format("[{0}] Iniciando download: {1}", tipo, nomeArq), Cores.AzulClaro);
			Stopwatch sw = Stopwatch.StartNew();
			try
			{
				using (WebClient client = new WebClient())
				{
					client.DownloadProgressChanged += delegate(object sender, DownloadProgressChangedEventArgs ev)
					{
						Invoke((Action)delegate
						{
							pb.Value = ev.ProgressPercentage;
							double num = (double)ev.BytesReceived / 1024.0 / 1024.0;
							double num2 = (double)ev.TotalBytesToReceive / 1024.0 / 1024.0;
							double num3 = ((sw.Elapsed.TotalSeconds > 0.0) ? (num / sw.Elapsed.TotalSeconds) : 0.0);
							SetStatus(string.Format("Baixando {0}: {1}% ({2:F1}/{3:F1} MB) @ {4:F1} MB/s", nomeArq, ev.ProgressPercentage, num, num2, num3));
						});
					};
					await client.DownloadFileTaskAsync(new Uri(urlFinal), destino);
				}
				sw.Stop();
				double totalMB = (double)new FileInfo(destino).Length / 1024.0 / 1024.0;
				lblStatus.ForeColor = Cores.Verde;
				lblStatus.Text = string.Format("✔ {0} baixado!", escolha);
				lblVersao.Text = string.Format("Versão local: {0}", escolha);
				SetStatus("Download concluído!");
				Log(string.Format("[{0}] Download concluído! {1:F1} MB em {2:F1}s → {3}", tipo, totalMB, sw.Elapsed.TotalSeconds, destino), Cores.Verde);
				MessageBox.Show(string.Format("Download concluído com sucesso!\n\nArquivo: {0}\nTamanho: {1:F1} MB\nSalvo em: {2}", nomeArq, totalMB, targetDir), "Download OK", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				DialogResult resExtrair = MessageBox.Show("O arquivo de atualização foi baixado com sucesso.\n\nDeseja extrair os arquivos automaticamente agora para finalizar a atualização?", "Extrair Atualização", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if (resExtrair == DialogResult.Yes && ExtrairArquivo(destino, targetDir, tipo))
				{
					MessageBox.Show("Atualização extraída e aplicada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
					DetectarVersaoProduto(tipo);
				}
			}
			catch (Exception ex)
			{
				lblStatus.ForeColor = Cores.Vermelho;
				lblStatus.Text = "✘ Erro no download";
				SetStatus("Erro: " + ex.Message);
				Log(string.Format("[{0}] ERRO: {1}", tipo, ex.Message), Cores.Vermelho);
				MessageBox.Show("Erro ao baixar:\n" + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			pb.Value = 100;
			btnAtual.Enabled = true;
		}

		private async Task<List<string>> ScanVersionsAsync(string baseUrl, string major, string minor, int startPatch, int count, bool isUp)
		{
			List<Task<KeyValuePair<string, bool>>> tasks = new List<Task<KeyValuePair<string, bool>>>();
			if (isUp)
			{
				int result = 0;
				if (!int.TryParse(minor, out result))
				{
					result = 9;
				}
				for (int i = result; i <= result + 8; i++)
				{
					int num = ((i == result) ? (startPatch + 1) : 0);
					int num2 = ((i == result) ? (startPatch + 30) : 30);
					for (int j = num; j <= num2; j++)
					{
						string versao = string.Format("{0}-{1}-{2}", major, i, j);
						string url = baseUrl + versao + ".rar";
						tasks.Add(Task.Run(async () => new KeyValuePair<string, bool>(value: await CheckUrlExistsAsync(url), key: versao)));
					}
				}
			}
			else
			{
				for (int k = 0; k <= count; k++)
				{
					int num3 = startPatch + k;
					string versao2 = string.Format("{0}-{1}", major, num3);
					string url2 = baseUrl + versao2 + ".rar";
					tasks.Add(Task.Run(async () => new KeyValuePair<string, bool>(value: await CheckUrlExistsAsync(url2), key: versao2)));
				}
			}
			return (from r in await Task.WhenAll(tasks)
				where r.Value
				select r.Key).OrderBy(delegate(string v)
			{
				string[] array = v.Split('-');
				if (isUp && array.Length == 3)
				{
					int result2;
					int num4 = (int.TryParse(array[1], out result2) ? result2 : 0);
					int result3;
					int num5 = (int.TryParse(array[2], out result3) ? result3 : 0);
					return num4 * 10000 + num5;
				}
				int result4;
				return (array.Length > 0 && int.TryParse(array[array.Length - 1], out result4)) ? result4 : 0;
			}).ToList();
		}

		private async Task<bool> CheckUrlExistsAsync(string url)
		{
			int num = default(int);
			int num2 = num;
			int num3 = 0;
			try
			{
				HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
				req.Method = "HEAD";
				req.Timeout = 4000;
				using (WebResponse resp = await Task.Factory.FromAsync((Func<AsyncCallback, object, IAsyncResult>)req.BeginGetResponse, (Func<IAsyncResult, WebResponse>)req.EndGetResponse, (object)null))
				{
					return ((HttpWebResponse)resp).StatusCode == HttpStatusCode.OK;
				}
			}
			catch
			{
				return false;
			}
		}

		private bool TryDetectLocalVersion(string tipo, out string major, out string minor, out string patch)
		{
			major = (minor = (patch = ""));
			try
			{
				string text = ((tipo == "UpSystem") ? pathUp : pathForca);
				if (!Directory.Exists(text))
				{
					return false;
				}
				string path = ((tipo == "UpSystem") ? "UpSystem.exe" : "ServidorForcaVendas.exe");
				string text2 = Path.Combine(text, path);
				if (!File.Exists(text2) && tipo == "ServidorForcaVendas")
				{
					string[] files = Directory.GetFiles(text, "ServidorForcaVendas*.exe");
					if (files.Length > 0)
					{
						text2 = files.OrderByDescending((string f) => new FileInfo(f).LastWriteTime).First();
					}
				}
				if (File.Exists(text2))
				{
					FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(text2);
					if (tipo == "UpSystem" && versionInfo.FileMajorPart > 0)
					{
						major = versionInfo.FileMajorPart.ToString();
						minor = versionInfo.FileBuildPart.ToString();
						patch = versionInfo.FilePrivatePart.ToString();
						if (minor != "0" || patch != "0")
						{
							return true;
						}
					}
					else if (tipo == "ServidorForcaVendas" && versionInfo.FileMajorPart > 0)
					{
						if (versionInfo.FileBuildPart > 0)
						{
							major = versionInfo.FileBuildPart.ToString();
							patch = versionInfo.FilePrivatePart.ToString();
						}
						else
						{
							major = versionInfo.FileMajorPart.ToString();
							patch = versionInfo.FilePrivatePart.ToString();
						}
						if (patch != "0" && patch != "")
						{
							return true;
						}
					}
				}
				string[] files2 = Directory.GetFiles(text, tipo + "*.rar");
				if (files2.Length > 0)
				{
					FileInfo fileInfo = (from f in files2
						select new FileInfo(f) into f
						orderby f.LastWriteTime descending
						select f).FirstOrDefault();
					if (fileInfo != null)
					{
						if (tipo == "UpSystem")
						{
							Match match = Regex.Match(fileInfo.Name, "UpSystem.*?(\\d+)[-.](\\d+)[-.](\\d+)", RegexOptions.IgnoreCase);
							if (match.Success)
							{
								major = match.Groups[1].Value;
								minor = match.Groups[2].Value;
								patch = match.Groups[3].Value;
								return true;
							}
						}
						else
						{
							Match match2 = Regex.Match(fileInfo.Name, "ServidorForcaVendas.*?(\\d+)[-.](\\d+)", RegexOptions.IgnoreCase);
							if (match2.Success)
							{
								major = match2.Groups[1].Value;
								patch = match2.Groups[2].Value;
								return true;
							}
						}
					}
				}
			}
			catch
			{
			}
			return false;
		}

		private bool ExtrairArquivo(string arquivoRar, string pastaDestino, string tipo)
		{
			string text = ((tipo == "UpSystem") ? "UpSystem" : "ServidorForcaVendas");
			try
			{
				Process[] processesByName = Process.GetProcessesByName(text);
				if (processesByName.Length > 0)
				{
					DialogResult dialogResult = MessageBox.Show(string.Format("O programa {0} está em execução. Ele precisa ser fechado para aplicar a atualização.\n\nDeseja encerrar o programa agora?", text), "Programa em Execução", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
					if (dialogResult != DialogResult.Yes)
					{
						Log(string.Format("[{0}] Extração cancelada: o processo está em execução.", tipo), Cores.Vermelho);
						MessageBox.Show("A extração foi cancelada porque o programa está em execução.", "Cancelado", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
						return false;
					}
					Process[] array = processesByName;
					foreach (Process process in array)
					{
						process.Kill();
						process.WaitForExit(5000);
					}
					Log(string.Format("[{0}] Processo encerrado para atualização.", tipo), Cores.AzulClaro);
				}
				if (tipo == "ServidorForcaVendas")
				{
					List<ServiceController> source = (from s in ServiceController.GetServices()
						where s.ServiceName.StartsWith("ForcaDeVendas", StringComparison.OrdinalIgnoreCase)
						select s).ToList();
					List<ServiceController> list = source.Where((ServiceController s) => s.Status != ServiceControllerStatus.Stopped).ToList();
					if (list.Count > 0)
					{
						using (FormSelecionarServicos formSelecionarServicos = new FormSelecionarServicos(list))
						{
							if (formSelecionarServicos.ShowDialog() != DialogResult.OK)
							{
								Log(string.Format("[{0}] Extração cancelada durante verificação de serviços.", tipo), Cores.Vermelho);
								MessageBox.Show("A extração foi cancelada pelo usuário.", "Cancelado", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
								return false;
							}
							foreach (ServiceController servicosSelecionado in formSelecionarServicos.ServicosSelecionados)
							{
								try
								{
									servicosSelecionado.Stop();
									servicosSelecionado.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10.0));
									Log(string.Format("[{0}] Serviço {1} parado com sucesso.", tipo, servicosSelecionado.ServiceName), Cores.AzulClaro);
								}
								catch (Exception ex)
								{
									Log(string.Format("[{0}] Erro ao parar serviço {1}: {2}", tipo, servicosSelecionado.ServiceName, ex.Message), Cores.Laranja);
								}
							}
						}
					}
				}
			}
			catch (Exception ex2)
			{
				Log(string.Format("[{0}] Erro ao tentar verificar/fechar processo: {1}", tipo, ex2.Message), Cores.Vermelho);
			}
			string text2 = "C:\\\\Program Files\\\\7-Zip\\\\7z.exe";
			string text3 = "C:\\\\Program Files\\\\WinRAR\\\\WinRAR.exe";
			string text4 = "C:\\\\Program Files\\\\WinRAR\\\\UnRAR.exe";
			string text5 = "";
			string text6 = "";
			if (File.Exists(text2))
			{
				text5 = text2;
				text6 = string.Format("x \"{0}\" -o\"{1}\" -y", arquivoRar, pastaDestino);
			}
			else if (File.Exists(text3))
			{
				text5 = text3;
				text6 = string.Format("x -ibck -y \"{0}\" \"{1}\\\"", arquivoRar, pastaDestino);
			}
			else
			{
				if (!File.Exists(text4))
				{
					string text7 = "Nenhuma ferramenta de extração (7-Zip ou WinRAR) encontrada nos caminhos padrão.\n\nPor favor, extraia o arquivo manualmente.";
					Log(string.Format("[{0}] Erro: 7-Zip/WinRAR não encontrados.", tipo), Cores.Vermelho);
					MessageBox.Show(text7, "Ferramenta não encontrada", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return false;
				}
				text5 = text4;
				text6 = string.Format("x -y \"{0}\" \"{1}\\\"", arquivoRar, pastaDestino);
			}
			try
			{
				Log(string.Format("[{0}] Extraindo arquivos usando {1}...", tipo, Path.GetFileName(text5)), Cores.AzulClaro);
				ProcessStartInfo processStartInfo = new ProcessStartInfo();
				processStartInfo.FileName = text5;
				processStartInfo.Arguments = text6;
				processStartInfo.CreateNoWindow = true;
				processStartInfo.UseShellExecute = false;
				processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
				ProcessStartInfo startInfo = processStartInfo;
				using (Process process2 = Process.Start(startInfo))
				{
					process2.WaitForExit(30000);
					if (process2.ExitCode == 0)
					{
						Log(string.Format("[{0}] Extração concluída com sucesso!", tipo), Cores.Verde);
						return true;
					}
					Log(string.Format("[{0}] Erro na extração. Código de saída: {1}", tipo, process2.ExitCode), Cores.Vermelho);
					return false;
				}
			}
			catch (Exception ex3)
			{
				Log(string.Format("[{0}] Falha na extração: {1}", tipo, ex3.Message), Cores.Vermelho);
				MessageBox.Show("Erro ao extrair arquivos:\n" + ex3.Message, "Erro de Extração", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return false;
			}
		}

		private void Log(string mensagem, Color cor)
		{
			if (rtbLog.InvokeRequired)
			{
				rtbLog.Invoke((Action)delegate
				{
					Log(mensagem, cor);
				});
				return;
			}
			string arg = DateTime.Now.ToString("HH:mm:ss");
			rtbLog.SelectionStart = rtbLog.TextLength;
			rtbLog.SelectionLength = 0;
			rtbLog.SelectionColor = Color.FromArgb(100, 140, 180);
			rtbLog.AppendText(string.Format("[{0}] ", arg));
			rtbLog.SelectionColor = cor;
			rtbLog.AppendText(mensagem + "\n");
			rtbLog.ScrollToCaret();
		}

		private void SetStatus(string texto)
		{
			if (lblStatusBar.InvokeRequired)
			{
				lblStatusBar.Invoke((Action)delegate
				{
					lblStatusBar.Text = texto;
				});
			}
			else
			{
				lblStatusBar.Text = texto;
			}
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			SetWindowRegion();
			if (btnFechar != null)
			{
				btnFechar.Location = new Point(base.Width - 40, 0);
			}
			if (btnMinimizar != null)
			{
				btnMinimizar.Location = new Point(base.Width - 78, 0);
			}
			Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			int num = 10;
			using (GraphicsPath graphicsPath = new GraphicsPath())
			{
				Rectangle rectangle = new Rectangle(1, 1, base.Width - 3, base.Height - 3);
				graphicsPath.AddArc(rectangle.X, rectangle.Y, num * 2, num * 2, 180f, 90f);
				graphicsPath.AddArc(rectangle.Right - num * 2, rectangle.Y, num * 2, num * 2, 270f, 90f);
				graphicsPath.AddArc(rectangle.Right - num * 2, rectangle.Bottom - num * 2, num * 2, num * 2, 0f, 90f);
				graphicsPath.AddArc(rectangle.X, rectangle.Bottom - num * 2, num * 2, num * 2, 90f, 90f);
				graphicsPath.CloseFigure();
				using (Pen pen = new Pen(Cores.NavyMedio, 2f))
				{
					e.Graphics.DrawPath(pen, graphicsPath);
				}
			}
		}

		private async void ChecarAtualizacaoSuporte()
		{
			try
			{
				HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL_VERSAO + "?t=" + DateTime.Now.Ticks);
				req.Timeout = 5000;
				using (WebResponse resp = await req.GetResponseAsync())
				using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
				{
					string versaoServidor = (await sr.ReadToEndAsync()).Trim();
					if (!string.IsNullOrEmpty(versaoServidor) && versaoServidor != VERSAO_ATUAL)
					{
						Version vServidor = new Version(versaoServidor);
						Version vAtual = new Version(VERSAO_ATUAL);
						if (vServidor > vAtual)
						{
							btnUpdateApp.Visible = true;
							Log("Uma nova versão do Suporte Infocenter (" + versaoServidor + ") está disponível!", Cores.Verde);
						}
					}
				}
			}
			catch
			{
				// Ignora erros de rede na checagem silenciosa
			}
		}

		private async void BtnUpdateApp_Click(object sender, EventArgs e)
		{
			btnUpdateApp.Enabled = false;
			btnUpdateApp.Text = "Baixando atualização... Aguarde.";
			try
			{
				string novoExe = Path.Combine(Application.StartupPath, "Suporte_Novo.exe");
				using (WebClient webClient = new WebClient())
				{
					await webClient.DownloadFileTaskAsync(new Uri(URL_EXE), novoExe);
				}

				string batPath = Path.Combine(Application.StartupPath, "update.bat");
				string batScript = "@echo off\r\n" +
								   "timeout /t 2 /nobreak > NUL\r\n" +
								   "del /f /q \"" + Application.ExecutablePath + "\"\r\n" +
								   "ren \"" + novoExe + "\" \"" + Path.GetFileName(Application.ExecutablePath) + "\"\r\n" +
								   "start \"\" \"" + Application.ExecutablePath + "\"\r\n" +
								   "del \"%~f0\"";
				File.WriteAllText(batPath, batScript);

				ProcessStartInfo psi = new ProcessStartInfo
				{
					FileName = batPath,
					UseShellExecute = true,
					WindowStyle = ProcessWindowStyle.Hidden
				};
				Process.Start(psi);
				Application.Exit();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Erro ao atualizar: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
				btnUpdateApp.Enabled = true;
				btnUpdateApp.Text = "NOVA VERSÃO DISPONÍVEL! CLIQUE PARA ATUALIZAR O SUPORTE INFOCENTER.";
			}
		}
	}
	internal class FormSelecionarVersao : Form
	{
		private ListBox lstVersoes;

		private BotaoRounded btnOk;

		private BotaoRounded btnCancelar;

		public string VersaoEscolhida { get; private set; }

		public FormSelecionarVersao(List<string> versoes, string tipo)
		{
			Text = "Selecionar Versão - " + tipo;
			base.Size = new Size(340, 280);
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.StartPosition = FormStartPosition.CenterParent;
			BackColor = Cores.FundoApp;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			Label label = new Label
			{
				Text = string.Format("Versões disponíveis para {0}:\n(A última é a mais recente)", tipo),
				Font = new Font("Segoe UI", 9.5f),
				ForeColor = Cores.TextoPrincipal,
				Location = new Point(12, 10),
				AutoSize = true
			};
			lstVersoes = new ListBox
			{
				Location = new Point(12, 55),
				Size = new Size(300, 140),
				Font = new Font("Segoe UI", 11f),
				BorderStyle = BorderStyle.FixedSingle,
				BackColor = Color.FromArgb(45, 45, 48),
				ForeColor = Color.White,
				SelectionMode = SelectionMode.One
			};
			lstVersoes.Items.AddRange(versoes.Cast<object>().ToArray());
			lstVersoes.SelectedIndex = lstVersoes.Items.Count - 1;
			btnOk = new BotaoRounded
			{
				Text = "Baixar Versão Selecionada",
				Location = new Point(12, 210),
				Size = new Size(195, 36),
				NormalColor = Cores.NavyMedio,
				HoverColor = Cores.AzulClaro,
				BackColor = Cores.NavyMedio
			};
			BotaoRounded botaoRounded = btnOk;
			EventHandler value = delegate
			{
				if (lstVersoes.SelectedItem != null)
				{
					VersaoEscolhida = lstVersoes.SelectedItem.ToString();
					base.DialogResult = DialogResult.OK;
					Close();
				}
			};
			botaoRounded.Click += value;
			btnCancelar = new BotaoRounded
			{
				Text = "Cancelar",
				Location = new Point(215, 210),
				Size = new Size(100, 36),
				NormalColor = Color.FromArgb(150, 160, 175),
				HoverColor = Color.FromArgb(120, 130, 145),
				BackColor = Color.FromArgb(150, 160, 175)
			};
			btnCancelar.Click += delegate
			{
				base.DialogResult = DialogResult.Cancel;
				Close();
			};
			base.Controls.AddRange(new Control[4] { label, lstVersoes, btnOk, btnCancelar });
		}
	}
	public class FormSelecionarServicos : Form
	{
		private CheckedListBox chkList;

		private BotaoRounded btnOk;

		private BotaoRounded btnCancelar;

		public List<ServiceController> ServicosSelecionados { get; private set; }

		public FormSelecionarServicos(List<ServiceController> servicos)
		{
			FormSelecionarServicos formSelecionarServicos = this;
			ServicosSelecionados = new List<ServiceController>();
			Text = "Serviços em Execução";
			base.Size = new Size(400, 300);
			base.StartPosition = FormStartPosition.CenterParent;
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			BackColor = Cores.FundoApp;
			ForeColor = Color.White;
			Label label = new Label
			{
				Text = "Selecione os serviços que deseja parar para a atualização:",
				Location = new Point(20, 20),
				AutoSize = true,
				Font = new Font("Segoe UI", 10f, FontStyle.Regular)
			};
			chkList = new CheckedListBox
			{
				Location = new Point(20, 50),
				Size = new Size(340, 140),
				BackColor = Cores.FundoCard,
				ForeColor = Color.White,
				Font = new Font("Segoe UI", 10f),
				BorderStyle = BorderStyle.FixedSingle,
				CheckOnClick = true
			};
			foreach (ServiceController servico in servicos)
			{
				chkList.Items.Add(servico.ServiceName, false);
			}
			btnOk = new BotaoRounded
			{
				Text = "Parar Selecionados",
				Location = new Point(45, 210),
				Size = new Size(160, 36),
				NormalColor = Cores.NavyMedio,
				HoverColor = Cores.AzulClaro,
				BackColor = Cores.NavyMedio
			};
			btnOk.Click += delegate
			{
				foreach (int checkedIndex in formSelecionarServicos.chkList.CheckedIndices)
				{
					formSelecionarServicos.ServicosSelecionados.Add(servicos[checkedIndex]);
				}
				formSelecionarServicos.DialogResult = DialogResult.OK;
				formSelecionarServicos.Close();
			};
			btnCancelar = new BotaoRounded
			{
				Text = "Cancelar",
				Location = new Point(215, 210),
				Size = new Size(110, 36),
				NormalColor = Color.FromArgb(150, 160, 175),
				HoverColor = Color.FromArgb(120, 130, 145),
				BackColor = Color.FromArgb(150, 160, 175)
			};
			btnCancelar.Click += delegate
			{
				base.DialogResult = DialogResult.Cancel;
				Close();
			};
			base.Controls.AddRange(new Control[4] { label, chkList, btnOk, btnCancelar });
		}
	}
	internal static class Program
	{
		[STAThread]
		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new FormPrincipal());
		}
	}
}
