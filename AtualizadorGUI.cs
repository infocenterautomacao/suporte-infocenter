﻿using System;
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
[assembly: AssemblyFileVersion("1.3.1.0")]
[assembly: AssemblyVersion("1.3.1.0")]
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
		private const string VERSAO_ATUAL = "1.3.1.0";
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

		private Panel pnlTabs;
		private BotaoRounded btnTabDownloads;
		private BotaoRounded btnTabLocal;
		private Panel pnlMainArea;
		private Panel pnlDownloads;
		private Panel pnlLocal;
		private CheckedListBox chkDownloads;
		private Label lblPastaDestino;
		private TextBox txtDestino;
		private BotaoRounded btnProcurar;
		private BotaoRounded btnBaixarSelecionados;
		private BotaoRounded btnAbrirLocal;
		private List<SistemaDownload> listaDownloadsDisponiveis = new List<SistemaDownload>();
		private const string ECARRINHO_URL = "https://www.ecarrinho.com/api/agent/version?channel=stable&platform=windows-x64";
		private const string GESYNC_URL = "https://download.geplug.com.br/GeSyncSetup.zip";

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
				BuscarVersoesPainelDownloads();
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
						lblUpStatus.Text = (ok ? "âœ” Local OK" : "âš  Não encontrado");
						lblUpStatus.ForeColor = (ok ? Cores.Verde : Cores.Vermelho);
						lblUpPasta.Text = "Pasta: " + pathUp;
					}
					else
					{
						string arg2 = (ok ? string.Format("{0}-{1}", maj, pat) : "Não detectada");
						lblForcaVersao.Text = (ok ? string.Format("Versão local: {0}", arg2) : "Nenhum arquivo local encontrado");
						lblForcaStatus.Text = (ok ? "âœ” Local OK" : "âš  Não encontrado");
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
				Text = "SUPORTE",
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
			btnMinimizar = CriarBotaoSistema("-", 14f);
			btnMinimizar.Location = new Point(base.Width - 78, 0);
			btnMinimizar.Size = new Size(38, 36);
			btnMinimizar.Click += delegate
			{
				base.WindowState = FormWindowState.Minimized;
			};
			btnFechar = CriarBotaoSistema("X", 13f);
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
				Text = "Suporte: Atualizado",
				Location = new Point(base.Width - 252, 24),
				Size = new Size(160, 32),
				NormalColor = Cores.Verde,
				HoverColor = Color.MediumSeaGreen,
				BackColor = Cores.NavyEscuro,
				ForeColor = Color.White,
				Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
				Visible = true,
				Cursor = Cursors.Hand
			};
			btnUpdateApp.Click += BtnUpdateApp_Click;

			pnlHeader.Controls.Add(btnUpdateApp);
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
				Text = "Versão " + VERSAO_ATUAL,
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
			ConstruirAbas();
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

				private void ConstruirAbas()
		{
			pnlTabs = new Panel
			{
				Location = new Point(0, 80),
				Size = new Size(680, 45),
				BackColor = Cores.FundoApp
			};
			
			btnTabDownloads = new BotaoRounded
			{
				Text = "Downloads",
				Location = new Point(20, 5),
				Size = new Size(150, 35),
				NormalColor = Cores.AzulClaro,
				HoverColor = Color.LightSkyBlue,
				BackColor = Cores.FundoApp,
				ForeColor = Color.White,
				Font = new Font("Segoe UI", 10f, FontStyle.Bold),
				Cursor = Cursors.Hand
			};
			btnTabDownloads.Click += (s, e) => AlternarAba(1);

			btnTabLocal = new BotaoRounded
			{
				Text = "Atualizador Local",
				Location = new Point(180, 5),
				Size = new Size(180, 35),
				NormalColor = Cores.NavyMedio,
				HoverColor = Color.FromArgb(70, 70, 90),
				BackColor = Cores.FundoApp,
				ForeColor = Color.White,
				Font = new Font("Segoe UI", 10f, FontStyle.Bold),
				Cursor = Cursors.Hand
			};
			btnTabLocal.Click += (s, e) => AlternarAba(0);
			
			btnTabRotinas = new BotaoRounded { Text = "Rotinas", Location = new Point(380, 5), Size = new Size(150, 35), NormalColor = Cores.NavyMedio, HoverColor = Color.FromArgb(70, 70, 90), BackColor = Cores.FundoApp, ForeColor = Color.White, Font = new Font("Segoe UI", 10f, FontStyle.Bold), Cursor = Cursors.Hand }; btnTabRotinas.Click += (s, e) => AlternarAba(2); pnlTabs.Controls.AddRange(new Control[3] { btnTabDownloads, btnTabLocal, btnTabRotinas });
			base.Controls.Add(pnlTabs);
			
			pnlMainArea = new Panel
			{
				Location = new Point(0, 125),
				Size = new Size(680, 245),
				BackColor = Cores.FundoApp
			};
			base.Controls.Add(pnlMainArea);
			
			pnlLocal = new Panel
			{
				Dock = DockStyle.Fill,
				BackColor = Cores.FundoApp,
				Visible = false
			};
			cardUp = CriarCard(new Point(20, 0), new Size(310, 245));
			cardForca = CriarCard(new Point(350, 0), new Size(310, 245));
			ConstruirCardUp();
			ConstruirCardForca();
			pnlLocal.Controls.AddRange(new Control[2] { cardUp, cardForca });
			
			pnlDownloads = new Panel
			{
				Dock = DockStyle.Fill,
				BackColor = Cores.FundoApp,
				Visible = true
			};
			ConstruirPainelDownloads();
			
			ConstruirPainelRotinas(); pnlMainArea.Controls.AddRange(new Control[3] { pnlDownloads, pnlLocal, pnlRotinas });
		}
		
		private void AlternarAba(int abaId)
		{
			pnlDownloads.Visible = (abaId == 1);
			pnlLocal.Visible = (abaId == 0);
			pnlRotinas.Visible = (abaId == 2);
			btnTabDownloads.NormalColor = (abaId == 1) ? Cores.AzulClaro : Cores.NavyMedio;
			btnTabLocal.NormalColor = (abaId == 0) ? Cores.AzulClaro : Cores.NavyMedio;
			btnTabRotinas.NormalColor = (abaId == 2) ? Cores.AzulClaro : Cores.NavyMedio;
			btnTabRotinas.Refresh();
			btnTabDownloads.Refresh();
			btnTabLocal.Refresh();
		}

				private BotaoRounded btnTabRotinas;
		private Panel pnlRotinas;
		private CheckedListBox chkRotinas;
		private BotaoRounded btnExecutarRotinas;

		private void ConstruirPainelRotinas()
		{
			pnlRotinas = new Panel
			{
				Dock = DockStyle.Fill,
				BackColor = Cores.FundoApp,
				Visible = false
			};
			
			Label lblTitulo = new Label
			{
				Text = "Quais rotinas deseja executar?",
				Location = new Point(20, 0),
				AutoSize = true,
				Font = new Font("Segoe UI", 10f, FontStyle.Bold),
				ForeColor = Color.White
			};
			
			chkRotinas = new CheckedListBox
			{
				Location = new Point(20, 25),
				Size = new Size(640, 150),
				BackColor = Cores.FundoCard,
				ForeColor = Color.White,
				Font = new Font("Segoe UI", 11f),
				BorderStyle = BorderStyle.FixedSingle,
				CheckOnClick = true
			};
			chkRotinas.Items.Add("Copiar Certificados e Imagens do ERP", false);
			chkRotinas.Items.Add("Permissões, Portas e Compartilhamento ERP", false);
			chkRotinas.Items.Add("Instalação do Força de Vendas como Serviço (NSSM)", false);
			
			btnExecutarRotinas = new BotaoRounded
			{
				Text = "Executar Selecionadas",
				Location = new Point(20, 190),
				Size = new Size(200, 35),
				NormalColor = Cores.Laranja,
				HoverColor = Color.Orange,
				BackColor = Cores.FundoApp,
				ForeColor = Color.White,
				Font = new Font("Segoe UI", 9f, FontStyle.Bold)
			};
			btnExecutarRotinas.Click += BtnExecutarRotinas_Click;
			
			pnlRotinas.Controls.AddRange(new Control[3] { lblTitulo, chkRotinas, btnExecutarRotinas });
		}
		
		private void BtnExecutarRotinas_Click(object sender, EventArgs e)
		{
			if (chkRotinas.CheckedItems.Count == 0)
			{
				MessageBox.Show("Selecione pelo menos uma rotina.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}
			
			foreach (object item in chkRotinas.CheckedItems)
			{
				string rotina = item.ToString();
				Log("Iniciando: " + rotina, Cores.TextoLog);
				
				if (rotina.Contains("Certificados e Imagens"))
				{
					RodarScriptEmbutido("COPIAR_CERT_IMAGENS_SERV_ERP.bat", ObterBatCopiarCertImagens());
				}
				else if (rotina.Contains("Permissões, Portas"))
				{
					RodarScriptEmbutido("PERMISSOES_PORTAS_COMPARTILHAR_ERP.bat", ObterBatPermissoesPortas());
				}
				else if (rotina.Contains("Força de Vendas como Serviço"))
				{
					ExecutarInstalacaoServicoNSSM();
				}
			}
		}
		
		private void RodarScriptEmbutido(string nomeArquivo, string conteudo)
		{
			string tempPath = Path.Combine(Path.GetTempPath(), nomeArquivo);
			try {
				File.WriteAllText(tempPath, conteudo, System.Text.Encoding.Default);
				ProcessStartInfo psi = new ProcessStartInfo {
					FileName = tempPath,
					UseShellExecute = true,
					Verb = "runas"
				};
				Process.Start(psi);
				Log("Executado com sucesso: " + nomeArquivo, Cores.Verde);
			} catch (Exception ex) {
				Log("Erro ao executar " + nomeArquivo + ": " + ex.Message, Color.Red);
			}
		}

		
		
		private string ObterBatCopiarCertImagens() {
			return @"@echo off
setlocal enabledelayedexpansion

:: Arquivos de configuracao
set ""CONF_INI=C:\UpSystem\CONF.INI""
set ""CONFIG_ARQUIVOS_INI=C:\UpSystem\CONFIGARQUIVOS.INI""

set ""SRV=""
set ""DIRETORIO_ORIGEM=""

echo ===================================================
echo     SCRIPT DE TRAZER CERTIFICADOS E IMAGENS
echo ===================================================
echo.

:: --- NOVIDADE AQUI: Solicitando diretorio de destino ---
set ""DESTINO_PADRAO=C:\UpSystem\""
set ""DESTINO=""

echo Digite o diretorio de destino ou pressione ENTER para usar o padrao:
echo [%DESTINO_PADRAO%]
set /p ""DESTINO=> ""

:: Se o usuario apenas apertou ENTER (variavel vazia), usa o padrao
if ""!DESTINO!""=="""" set ""DESTINO=!DESTINO_PADRAO!""

:: Garante que o diretorio de destino termine sempre com barra invertida (\)
if not ""!DESTINO:~-1!""==""\"" set ""DESTINO=!DESTINO!\""

:: Cria a pasta de destino caso o usuario tenha digitado um caminho que ainda nao existe
if not exist ""!DESTINO!"" (
    echo.
    echo [INFO] Criando diretorio de destino !DESTINO!...
    mkdir ""!DESTINO!""
)

echo.
:: 1. Busca o IP no CONF.INI
if exist ""%CONF_INI%"" (
    for /f ""tokens=1,* delims=="" %%a in ('type ""%CONF_INI%"" ^| findstr /b /i ""SRV=""') do (
        set ""SRV=%%b""
    )
) else (
    echo [ERRO] Arquivo %CONF_INI% nao encontrado.
    pause
    exit /b
)

:: Limpa possiveis espacos em branco do valor capturado
set ""SRV=%SRV: =%""

:: 2. Verifica se o servidor e localhost
if /i ""%SRV%""==""localhost"" (
    echo [INFO] Servidor local detectado. Buscando caminho no CONFIGARQUIVOS.INI...
    if exist ""%CONFIG_ARQUIVOS_INI%"" (
        for /f ""tokens=1,* delims=="" %%a in ('type ""%CONFIG_ARQUIVOS_INI%"" ^| findstr /b /i ""exeremoto=""') do (
            set ""LINHA_EXE=%%b""
            :: Remove espacos em branco da linha caso existam
            set ""LINHA_EXE=!LINHA_EXE: =!""
            :: Extrai dinamicamente apenas a pasta do executavel
            for %%I in (""!LINHA_EXE!"") do set ""DIRETORIO_ORIGEM=%%~dpI""
        )
    ) else (
        echo [ERRO] Arquivo %CONFIG_ARQUIVOS_INI% nao encontrado.
        pause
        exit /b
    )
) else (
    :: Se nao for localhost, monta o caminho de rede padrao
    echo [INFO] Servidor de rede detectado: %SRV%
    set ""DIRETORIO_ORIGEM=\\%SRV%\upsystem\""
)

:: Verifica se a origem foi definida corretamente
if ""!DIRETORIO_ORIGEM!""=="""" (
    echo [ERRO] Nao foi possivel determinar o diretorio de origem.
    pause
    exit /b
)

echo.
echo [ORIGEM]  !DIRETORIO_ORIGEM!
echo [DESTINO] !DESTINO!
echo.
echo Iniciando a copia dos arquivos .pfx, .png, .jpg e .jpeg...
echo.

:: 3. Copia todos os arquivos da origem para o destino
xcopy /Y /D /C ""!DIRETORIO_ORIGEM!*.pfx"" ""!DESTINO!"" 2>nul
xcopy /Y /D /C ""!DIRETORIO_ORIGEM!*.png"" ""!DESTINO!"" 2>nul
xcopy /Y /D /C ""!DIRETORIO_ORIGEM!*.jpg"" ""!DESTINO!"" 2>nul
xcopy /Y /D /C ""!DIRETORIO_ORIGEM!*.jpeg"" ""!DESTINO!"" 2>nul

echo.
echo [SUCESSO] Varredura e copia concluidas.
echo.
pause";
		}

		private string ObterBatPermissoesPortas() {
			return @"title by: Yago Rocha - Infocenter Automacao
@echo off
chcp 65001
setlocal

:: Defina o caminho da pasta
set ""Pasta=C:\UpSystem""

:: Verifique se a pasta existe
if not exist ""%Pasta%"" (
    echo [ERRO] A pasta %Pasta% não existe.
    pause
    exit /b
)

:: Conceda controle total para os grupos e usuários especificados
echo Concedendo controle total para ""Todos""...
icacls ""%Pasta%"" /grant ""Todos:(OI)(CI)F"" /T /C >nul 2>&1

echo Concedendo controle total para ""Usuários""...
icacls ""%Pasta%"" /grant ""Usuários:(OI)(CI)F"" /T /C >nul 2>&1

echo Concedendo controle total para ""Rede""...
icacls ""%Pasta%"" /grant ""Rede:(OI)(CI)F"" /T /C >nul 2>&1

echo Concedendo controle total para ""Administrador""...
icacls ""%Pasta%"" /grant ""Administrador:(OI)(CI)F"" /T /C >nul 2>&1

echo Concedendo controle total para ""Administradores""...
icacls ""%Pasta%"" /grant ""Administradores:(OI)(CI)F"" /T /C >nul 2>&1

echo Concedendo controle total para ""SISTEMA""...
icacls ""%Pasta%"" /grant ""SISTEMA:(OI)(CI)F"" /T /C >nul 2>&1

echo Concedendo controle total para ""Usuários autenticados""...
icacls ""%Pasta%"" /grant ""Usuários autenticados:(OI)(CI)F"" /T /C >nul 2>&1

:: Remova a herança de permissões
echo Removendo herança de permissões da pasta %Pasta%...
icacls ""%Pasta%"" /inheritance:r

:: Verifique o status da execução
if errorlevel 1 (
    echo [ERRO] Falha ao aplicar permissões.
) else (
    echo Permissões aplicadas com sucesso.
)

echo --------------------------------------------------------------------------------------

Echo Compartilhando pasta do sistema

net share UpSystem=C:\UpSystem /grant:Todos,FULL

echo --------------------------------------------------------------------------------------


:: Adiciona regras do firewall
echo Adicionando regras do firewall...

for %%P in (3050 8082 2018 7079 1080) do (
    netsh advfirewall firewall add rule name=""up-TCP-%%P"" action=allow protocol=TCP dir=in localport=%%P >nul 2>&1
    netsh advfirewall firewall add rule name=""up-UDP-%%P"" action=allow protocol=UDP dir=in localport=%%P >nul 2>&1
    netsh advfirewall firewall add rule name=""up-TCP-%%P"" action=allow protocol=TCP dir=out localport=%%P >nul 2>&1
    netsh advfirewall firewall add rule name=""up-UDP-%%P"" action=allow protocol=UDP dir=out localport=%%P >nul 2>&1
)

echo Regras do firewall adicionadas com sucesso.


echo --------------------------------------------------------------------------------------


:: Adiciona exclusões ao Windows Defender
echo Adicionando exclusões ao Windows Defender...

powershell -Command Add-MpPreference -ExclusionPath 'C:\Program Files (x86)\MasterRemote','C:\ProgramData\MasterRemote','C:\UpSystem'

echo Exclusões do Windows Defender adicionadas com sucesso.


echo --------------------------------------------------------------------------------------


:: Configurações de falha dos serviços
echo Configurando ações de falha dos serviços...

:: Defina os nomes dos serviços em uma lista
set ""services=FirebirdServerDefaultInstance Spooler""

:: Loop para aplicar as mesmas configurações de falha para todos os serviços
for %%S in (%services%) do (
    echo Configurando ações de falha para %%S...
    sc failure ""%%S"" reset= 86400 actions= restart/60000/restart/60000/restart/60000
)

echo Configurações de falha aplicadas com sucesso para todos os serviços.


echo --------------------------------------------------------------------------------------




echo Após a execução do comando, verifique manualmente as permissões na pasta do sistema para garantir que todas as permissões foram aplicadas conforme esperado.
echo Lembre também de abrir o suporte.exe se caso tenha instalado o sistema nesse instante.



echo --------------------------------------------------------------------------------------


pause
endlocal
";
		}


		private void RodarBatAdminVelho(string batPath)
		{
			if (!File.Exists(batPath))
			{
				Log("ERRO: Arquivo " + batPath + " não encontrado.", Color.Red);
				return;
			}
			
			try {
				ProcessStartInfo psi = new ProcessStartInfo {
					FileName = batPath,
					UseShellExecute = true,
					Verb = "runas"
				};
				Process.Start(psi);
				Log("Executado com sucesso: " + Path.GetFileName(batPath), Cores.Verde);
			} catch (Exception ex) {
				Log("Erro ao executar " + Path.GetFileName(batPath) + ": " + ex.Message, Color.Red);
			}
		}
		
		
		private void ExtrairRecursoSeNaoExistir(string resourceName, string destPath)
		{
			if (File.Exists(destPath)) return;
			try {
				using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
				{
					if (stream != null)
					{
						using (FileStream fileStream = new FileStream(destPath, FileMode.Create))
						{
							byte[] buffer = new byte[8192];
							int bytesRead;
							while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
							{
								fileStream.Write(buffer, 0, bytesRead);
							}
						}
						Log("Arquivo " + resourceName + " extraido com sucesso.", Cores.Verde);
					}
				}
			} catch (Exception ex) {
				Log("Erro ao extrair " + resourceName + ": " + ex.Message, Color.Red);
			}
		}

		
		private void BaixarDependencia(string url, string destino)
		{
			if (File.Exists(destino)) return;
			try {
				using (WebClient wc = new WebClient())
				{
					// Bypass SSL se necessario
					ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
					wc.DownloadFile(url, destino);
					Log("Dependencia baixada: " + Path.GetFileName(destino), Cores.Verde);
				}
			} catch (Exception ex) {
				Log("Erro ao baixar " + Path.GetFileName(destino) + ": " + ex.Message, Color.Red);
			}
		}

		private void ExecutarInstalacaoServicoNSSM()

		{
			using(FormServicoForcaVendas frm = new FormServicoForcaVendas())
			{
				if (frm.ShowDialog() == DialogResult.OK)
				{
					
  					
  					
  					string dir = frm.DiretorioSelecionado;
  					string srv = frm.NomeServico;
  					string batPath = Path.Combine(dir, "install_service_nssm.bat");
  					
					string urlNssm = "https://raw.githubusercontent.com/infocenterautomacao/suporte-infocenter/main/instalar%20for%C3%A7a%20de%20vendas%20servi%C3%A7o/nssm.exe";
					string urlBat = "https://raw.githubusercontent.com/infocenterautomacao/suporte-infocenter/main/instalar%20for%C3%A7a%20de%20vendas%20servi%C3%A7o/install_service_nssm.bat";
					
					BaixarDependencia(urlNssm, Path.Combine(dir, "nssm.exe"));
					BaixarDependencia(urlBat, batPath);



					
					if (!File.Exists(batPath)) {
						MessageBox.Show("Arquivo " + batPath + " não encontrado na pasta especificada!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}
					
					try {
						string conteudo = File.ReadAllText(batPath, System.Text.Encoding.Default);
						conteudo = Regex.Replace(conteudo, "set\\s+\"SERVICO=[^\"]*\"", "set \"SERVICO=" + srv + "\"");
						File.WriteAllText(batPath, conteudo, System.Text.Encoding.Default);
						Log("Nome do serviço alterado no arquivo .bat para: " + srv, Cores.TextoLog);
					} catch (Exception ex) {
						MessageBox.Show("Erro ao alterar o .bat: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}
					
					try {
						ProcessStartInfo psi = new ProcessStartInfo {
							FileName = "cmd.exe",
							Arguments = "/k \"\"" + batPath + "\" \"" + dir + "\"\"",
							UseShellExecute = true,
							Verb = "runas"
						};
						Process.Start(psi);
						Log("Comando de instalação enviado para CMD Admin.", Cores.Verde);
					} catch (Exception ex) {
						Log("Erro ao iniciar CMD: " + ex.Message, Color.Red);
					}
				}
			}
		}


		private void ConstruirPainelDownloads()
		{
			Label lblTitulo = new Label
			{
				Text = "O que deseja baixar?",
				Location = new Point(20, 0),
				AutoSize = true,
				Font = new Font("Segoe UI", 10f, FontStyle.Bold),
				ForeColor = Color.White
			};
			
			chkDownloads = new CheckedListBox
			{
				Location = new Point(20, 25),
				Size = new Size(640, 150),
				BackColor = Cores.FundoCard,
				ForeColor = Color.White,
				Font = new Font("Segoe UI", 10f),
				BorderStyle = BorderStyle.FixedSingle,
				CheckOnClick = true
			};
			
			lblPastaDestino = new Label
			{
				Text = "Pasta de destino:",
				Location = new Point(20, 185),
				AutoSize = true,
				Font = new Font("Segoe UI", 9f),
				ForeColor = Color.White
			};
			
			txtDestino = new TextBox
			{
				Location = new Point(20, 210),
				Size = new Size(340, 25),
				Font = new Font("Segoe UI", 10f),
				BackColor = Cores.FundoApp,
				ForeColor = Color.White,
				BorderStyle = BorderStyle.FixedSingle,
				Text = Path.Combine(Application.StartupPath, "Downloads")
			};
			
			btnProcurar = new BotaoRounded
			{
				Text = "Procurar...",
				Location = new Point(370, 207),
				Size = new Size(90, 30),
				NormalColor = Cores.NavyMedio,
				HoverColor = Cores.AzulClaro,
				BackColor = Cores.FundoApp,
				ForeColor = Color.White,
				Font = new Font("Segoe UI", 9f)
			};
			btnProcurar.Click += (s, e) => {
				using (FolderBrowserDialog fbd = new FolderBrowserDialog()) {
					if (fbd.ShowDialog() == DialogResult.OK) {
						txtDestino.Text = fbd.SelectedPath;
					}
				}
			};
			
			btnBaixarSelecionados = new BotaoRounded
			{
				Text = "Baixar selecionados",
				Location = new Point(470, 207),
				Size = new Size(130, 30),
				NormalColor = Cores.Verde,
				HoverColor = Color.LightGreen,
				BackColor = Cores.FundoApp,
				ForeColor = Color.White,
				Font = new Font("Segoe UI", 9f, FontStyle.Bold)
			};
			btnBaixarSelecionados.Click += BtnBaixarSelecionados_Click;
			
			pnlDownloads.Controls.AddRange(new Control[6] { lblTitulo, chkDownloads, lblPastaDestino, txtDestino, btnProcurar, btnBaixarSelecionados });
		}
		
		private async void BtnBaixarSelecionados_Click(object sender, EventArgs e)
		{
			if (chkDownloads.CheckedItems.Count == 0)
			{
				MessageBox.Show("Selecione pelo menos um item para baixar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}
			
			string pastaDestino = txtDestino.Text;
			if (!Directory.Exists(pastaDestino))
			{
				try { Directory.CreateDirectory(pastaDestino); }
				catch { MessageBox.Show("Erro ao criar a pasta de destino.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
			}
			
			btnBaixarSelecionados.Enabled = false;
			foreach (object item in chkDownloads.CheckedItems)
			{
				SistemaDownload sys = item as SistemaDownload;
				if (sys != null)
				{
					Log("Baixando " + sys.Nome + "...", Cores.Verde);
					try
					{
						using (WebClient wc = new WebClient())
						{
							string arquivoSalvo = Path.Combine(pastaDestino, sys.ArquivoNome);
							await wc.DownloadFileTaskAsync(new Uri(sys.UrlDownload), arquivoSalvo);
							Log("OK - " + sys.Nome + " salvo em: " + arquivoSalvo, Cores.Laranja);
						}
					}
					catch (Exception ex)
					{
						Log("ERRO ao baixar " + sys.Nome + ": " + ex.Message, Color.Red);
					}
				}
			}
			btnBaixarSelecionados.Enabled = true;
			Log("Download de todos os itens selecionados concluído.", Cores.Verde);
		}

		private async void BuscarVersoesPainelDownloads()
		{
			try
			{
				Log("Buscando versões mais recentes para Downloads...", Cores.TextoLog);
				listaDownloadsDisponiveis.Clear();
				chkDownloads.Items.Clear();
				
				// 1. UpSystem, ForcaVendas e MobileUpWin via codigoup
				HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.codigoup.com/painel/files/");
				req.Timeout = 10000;
				using (WebResponse resp = await req.GetResponseAsync())
				using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
				{
					string html = await sr.ReadToEndAsync();
					string pattern = "<a href=\"([^\"]+\\.rar)\">[^<]*</a>\\s*</td>\\s*<td[^>]*>\\s*(\\d{4}-\\d{2}-\\d{2}\\s+\\d{2}:\\d{2})\\s*</td>\\s*<td[^>]*>\\s*([^<]+?)\\s*</td>";
					MatchCollection matches = Regex.Matches(html, pattern);
					
					var dict = new Dictionary<string, Tuple<string, DateTime, string, string>>();
					foreach (Match m in matches)
					{
						string href = m.Groups[1].Value;
						string nomeDecodado = Uri.UnescapeDataString(href);
						DateTime dataMod = DateTime.ParseExact(m.Groups[2].Value, "yyyy-MM-dd HH:mm", null);
						string tamanho = m.Groups[3].Value;
						
						string prefixo = null;
						if (nomeDecodado.StartsWith("UpSystem")) prefixo = "UpSystem";
						else if (nomeDecodado.StartsWith("ServidorForcaVendas")) prefixo = "ServidorForcaVendas";
						else if (nomeDecodado.StartsWith("MobileUpWin")) prefixo = "MobileUpWin";
						
						if (prefixo != null)
						{
							if (!dict.ContainsKey(prefixo) || dict[prefixo].Item2 < dataMod)
							{
								dict[prefixo] = Tuple.Create("https://www.codigoup.com/painel/files/" + href, dataMod, tamanho, nomeDecodado);
							}
						}
					}
					
					string[] order = new string[] { "UpSystem", "ServidorForcaVendas", "MobileUpWin" };
					foreach (string prefixo in order)
					{
						if (dict.ContainsKey(prefixo))
						{
							var info = dict[prefixo];
							listaDownloadsDisponiveis.Add(new SistemaDownload {
								Nome = prefixo,
								ArquivoNome = info.Item4,
								UrlDownload = info.Item1,
								Exibicao = info.Item3 + ", " + info.Item2.ToString("dd/MM/yyyy")
							});
						}
					}
				}
				
				// 2. eHub API
				try {
					HttpWebRequest reqEh = (HttpWebRequest)WebRequest.Create(ECARRINHO_URL);
					reqEh.Timeout = 5000;
					using (WebResponse respEh = await reqEh.GetResponseAsync())
					using (StreamReader srEh = new StreamReader(respEh.GetResponseStream()))
					{
						string json = await srEh.ReadToEndAsync();
						Match mVer = Regex.Match(json, "\"latest_version\"\\s*:\\s*\"([^\"]+)\"");
						Match mUrl = Regex.Match(json, "\"download_url\"\\s*:\\s*\"([^\"]+)\"");
						Match mSize = Regex.Match(json, "\"size_bytes\"\\s*:\\s*(\\d+)");
						
						if (mVer.Success && mUrl.Success)
						{
							string url = mUrl.Groups[1].Value.Replace("\\/", "/");
							string version = mVer.Groups[1].Value;
							long size = mSize.Success ? long.Parse(mSize.Groups[1].Value) : 0;
							string sizeStr = size > 0 ? (size / 1048576.0).ToString("N1") + " MB" : "";
							
							listaDownloadsDisponiveis.Add(new SistemaDownload {
								Nome = "eHub",
								ArquivoNome = Path.GetFileName(new Uri(url).LocalPath),
								UrlDownload = url,
								Exibicao = "v" + version + (sizeStr != "" ? ", " + sizeStr : "")
							});
						}
					}
				} catch (Exception ex) { Log("Aviso: Falha ao checar eHub: " + ex.Message, Cores.Laranja); }
				
				// 3. GeSync
				try {
					HttpWebRequest reqGe = (HttpWebRequest)WebRequest.Create(GESYNC_URL);
					reqGe.Method = "HEAD";
					reqGe.Timeout = 5000;
					using (WebResponse respGe = await reqGe.GetResponseAsync())
					{
						long contentLength = respGe.ContentLength;
						string sizeStr = contentLength > 0 ? (contentLength / 1048576.0).ToString("N1") + " MB" : "";
						
						string dateStr = "";
						string lastMod = respGe.Headers["Last-Modified"];
						if (!string.IsNullOrEmpty(lastMod))
						{
							DateTime dt;
							if (DateTime.TryParse(lastMod, out dt))
								dateStr = dt.ToString("dd/MM/yyyy");
						}
						
						listaDownloadsDisponiveis.Add(new SistemaDownload {
							Nome = "GeSync",
							ArquivoNome = "GeSyncSetup.zip",
							UrlDownload = GESYNC_URL,
							Exibicao = sizeStr + (dateStr != "" ? " (" + dateStr + ")" : "")
						});
					}
				} catch (Exception ex) { Log("Aviso: Falha ao checar GeSync: " + ex.Message, Cores.Laranja); }
				
				foreach (var sys in listaDownloadsDisponiveis)
				{
					chkDownloads.Items.Add(sys, true);
				}
				
			}
			catch (Exception ex)
			{
				Log("Erro ao buscar versões para downloads: " + ex.Message, Color.Red);
			}
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
				Visible = true,
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
				Visible = true,
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
				lblStatus.Text = "âš  Versão não detectada localmente";
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
				lblStatus.Text = "âœ” Sistema atualizado!";
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
			lblStatus.Text = "â¬‡ Baixando...";
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
				lblStatus.Text = string.Format("âœ” {0} baixado!", escolha);
				lblVersao.Text = string.Format("Versão local: {0}", escolha);
				SetStatus("Download concluído!");
				Log(string.Format("[{0}] Download concluído! {1:F1} MB em {2:F1}s â†’ {3}", tipo, totalMB, sw.Elapsed.TotalSeconds, destino), Cores.Verde);
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
				lblStatus.Text = "âœ˜ Erro no download";
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
							btnUpdateApp.NormalColor = Color.Crimson; 
							btnUpdateApp.HoverColor = Color.Red;
							btnUpdateApp.Text = "Atualização Disponível!";
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
			btnUpdateApp.Text = "Buscando...";
			try
			{
				HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL_VERSAO + "?t=" + DateTime.Now.Ticks);
				req.Timeout = 5000;
				string versaoServidor = "";
				using (WebResponse resp = await req.GetResponseAsync())
				using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
				{
					versaoServidor = (await sr.ReadToEndAsync()).Trim();
				}

				if (!string.IsNullOrEmpty(versaoServidor) && versaoServidor != VERSAO_ATUAL)
				{
					Version vServidor = new Version(versaoServidor);
					Version vAtual = new Version(VERSAO_ATUAL);
					if (vServidor > vAtual)
					{
						btnUpdateApp.Text = "Nova versão (" + versaoServidor + ") encontrada! Baixando...";
						
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
						return;
					}
				}
				
				MessageBox.Show("Você já está utilizando a versão mais recente (" + VERSAO_ATUAL + ") do Suporte Infocenter.", "Atualizado", MessageBoxButtons.OK, MessageBoxIcon.Information);
				btnUpdateApp.Enabled = true;
				btnUpdateApp.Text = "Suporte: Atualizado";
			}
			catch (Exception ex)
			{
				MessageBox.Show("Não foi possível verificar/baixar atualizações. Erro: " + ex.Message, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				btnUpdateApp.Enabled = true;
				btnUpdateApp.Text = "Suporte: Atualizado";
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
		internal class FormServicoForcaVendas : Form
	{
		public string DiretorioSelecionado { get; private set; }
		public string NomeServico { get; private set; }
		
		private TextBox txtDiretorio;
		private TextBox txtNomeServico;
		
		public FormServicoForcaVendas()
		{
			Text = "Configurar Serviço - Força de Vendas";
			Size = new Size(520, 420);
			StartPosition = FormStartPosition.CenterParent;
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false; MinimizeBox = false;
			BackColor = Color.FromArgb(28, 28, 28);
			ForeColor = Color.White;
			
			string existentes = DetectarServicosExistentes();
			
			Label lblInfo = new Label {
				Text = "Serviços encontrados no Windows:\n" + existentes,
				Location = new Point(20, 20),
				AutoSize = true,
				Font = new Font("Segoe UI", 9f),
				ForeColor = Color.FromArgb(255, 165, 0)
			};
			
			Label lblDir = new Label {
				Text = "Diretório do Força de Vendas:",
				Location = new Point(20, 150), AutoSize = true, Font = new Font("Segoe UI", 10f)
			};
			txtDiretorio = new TextBox {
				Location = new Point(20, 175), Size = new Size(350, 25), Font = new Font("Segoe UI", 10f),
				Text = "C:\\UpSystem\\ForcaVendas",
				BackColor = Color.FromArgb(45, 45, 48), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle
			};
			Button btnProcurar = new Button {
				Text = "Procurar...", Location = new Point(380, 173), Size = new Size(90, 28),
				BackColor = Color.FromArgb(0, 122, 204), ForeColor = Color.White, FlatStyle = FlatStyle.Flat
			};
			btnProcurar.FlatAppearance.BorderSize = 0;
			btnProcurar.Click += (s, e) => {
				using(FolderBrowserDialog fbd = new FolderBrowserDialog()) {
					if (fbd.ShowDialog() == DialogResult.OK) txtDiretorio.Text = fbd.SelectedPath;
				}
			};
			
			Label lblNome = new Label {
				Text = "Nome desejado para o Serviço:",
				Location = new Point(20, 215), AutoSize = true, Font = new Font("Segoe UI", 10f)
			};
			txtNomeServico = new TextBox {
				Location = new Point(20, 240), Size = new Size(350, 25), Font = new Font("Segoe UI", 10f),
				Text = "ForcaDeVendas",
				BackColor = Color.FromArgb(45, 45, 48), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle
			};
			
			Button btnOk = new Button {
				Text = "Confirmar e Instalar", Location = new Point(20, 300), Size = new Size(180, 40),
				BackColor = Color.MediumSeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
				Font = new Font("Segoe UI", 10f, FontStyle.Bold)
			};
			btnOk.FlatAppearance.BorderSize = 0;
			btnOk.Click += (s, e) => {
				if (string.IsNullOrWhiteSpace(txtNomeServico.Text) || string.IsNullOrWhiteSpace(txtDiretorio.Text)) {
					MessageBox.Show("Preencha todos os campos.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}
				DiretorioSelecionado = txtDiretorio.Text;
				NomeServico = txtNomeServico.Text;
				DialogResult = DialogResult.OK;
				Close();
			};
			
			Controls.AddRange(new Control[] { lblInfo, lblDir, txtDiretorio, btnProcurar, lblNome, txtNomeServico, btnOk });
		}
		
		private string DetectarServicosExistentes()
		{
			try {
				int count = 0;
				string lista = "";
				foreach(System.ServiceProcess.ServiceController sc in System.ServiceProcess.ServiceController.GetServices()) {
					if (sc.ServiceName.IndexOf("Forca", StringComparison.OrdinalIgnoreCase) >= 0) {
						count++;
						lista += "- " + sc.ServiceName + " (" + sc.Status + ")\n";
					}
				}
				if (count == 0) return "Nenhum serviço com a palavra 'Forca' encontrado.";
				return "Total encontrados: " + count + "\n\n" + lista;
			} catch { return "Não foi possível detectar."; }
		}
	}


	internal class SistemaDownload
	{
		public string Nome { get; set; }
		public string ArquivoNome { get; set; }
		public string UrlDownload { get; set; }
		public string Exibicao { get; set; }
		
		public override string ToString()
		{
			return Nome + " - " + ArquivoNome + " (" + Exibicao + ")";
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




