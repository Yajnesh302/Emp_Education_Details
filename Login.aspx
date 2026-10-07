<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="EducationApp.Login" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
    <title>Login - Employee Education Portal</title>

    <!-- Local Offline CSS Assets -->
    <link href="Static/fontawesome-free/css/all.min.css" rel="stylesheet" type="text/css" />
    <link href="Static/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="Static/css/sb-admin-2.min.css" rel="stylesheet" type="text/css" />
    <link href="Static/css/custom-education.css?v=20261007_02" rel="stylesheet" type="text/css" />

    <style>
        html, body {
            height: 100%;
            margin: 0;
            padding: 0;
        }
        body {
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            background: radial-gradient(circle at 50% 20%, #1e3a8a 0%, #0f172a 65%, #080c14 100%);
            font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif;
            color: #334155;
            padding: 24px 16px;
        }
        form#form1 {
            width: 100%;
            display: flex;
            align-items: center;
            justify-content: center;
        }
        .login-wrapper {
            width: 100%;
            max-width: 920px;
            margin: auto;
        }
        .login-card {
            background: #ffffff;
            border-radius: 18px;
            box-shadow: 0 25px 60px -15px rgba(0, 8, 28, 0.55), 0 0 0 1px rgba(255, 255, 255, 0.08);
            overflow: hidden;
            border: none;
        }
        .login-brand-panel {
            background: linear-gradient(165deg, #f8fafc 0%, #edf2f7 100%);
            border-right: 1px solid #e2e8f0;
            padding: 50px 36px;
            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: center;
            text-align: center;
            position: relative;
        }
        .login-brand-panel::before {
            content: "";
            position: absolute;
            top: 0; left: 0; right: 0;
            height: 5px;
            background: linear-gradient(90deg, #1e40af, #3b82f6);
        }
        .brand-logo-img {
            max-width: 220px;
            width: 100%;
            height: auto;
            margin-bottom: 24px;
            filter: drop-shadow(0 4px 12px rgba(15, 23, 42, 0.08));
        }
        .brand-title {
            font-size: 1.25rem;
            font-weight: 800;
            color: #0f172a;
            letter-spacing: 0.5px;
            margin-bottom: 8px;
            line-height: 1.3;
        }
        .brand-subtitle {
            font-size: 0.85rem;
            color: #64748b;
            font-weight: 600;
            margin-bottom: 24px;
            line-height: 1.45;
        }
        .badge-auth-security {
            display: inline-flex;
            align-items: center;
            padding: 8px 16px;
            border-radius: 9999px;
            background: #e0f2fe;
            border: 1px solid #bae6fd;
            color: #0369a1;
            font-size: 0.8rem;
            font-weight: 600;
        }
        .login-form-panel {
            padding: 48px 44px;
            display: flex;
            flex-direction: column;
            justify-content: center;
            background: #ffffff;
        }
        @media (max-width: 576px) {
            .login-form-panel {
                padding: 32px 24px;
            }
        }
        .form-title {
            font-size: 1.5rem;
            font-weight: 800;
            color: #0f172a;
            margin-bottom: 4px;
        }
        .form-subtitle {
            font-size: 0.88rem;
            color: #64748b;
            margin-bottom: 28px;
        }
        .form-label-bold {
            font-size: 0.85rem;
            font-weight: 700;
            color: #334155;
            margin-bottom: 6px;
            display: block;
        }
        .form-label-bold .req {
            color: #ef4444;
        }
        .input-group-auth {
            border: 1.5px solid #cbd5e1;
            border-radius: 10px;
            overflow: hidden;
            transition: all 0.2s ease;
            background: #f8fafc;
        }
        .input-group-auth:focus-within {
            border-color: #2563eb;
            box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.15);
            background: #ffffff;
        }
        .input-group-auth .input-group-text {
            background: transparent;
            border: none;
            color: #64748b;
            padding-left: 14px;
            padding-right: 10px;
            font-size: 0.95rem;
        }
        .input-group-auth .form-control {
            border: none;
            background: transparent;
            font-size: 0.92rem;
            color: #0f172a;
            height: 44px;
            box-shadow: none !important;
            padding-left: 4px;
        }
        .input-group-auth .form-control::placeholder {
            color: #94a3b8;
            font-size: 0.88rem;
        }
        .btn-toggle-pwd {
            background: transparent;
            border: none;
            color: #94a3b8;
            padding: 0 14px;
            cursor: pointer;
            transition: color 0.15s ease;
        }
        .btn-toggle-pwd:hover {
            color: #334155;
        }
        .btn-submit-auth {
            height: 46px;
            border-radius: 10px;
            background: linear-gradient(135deg, #1e40af 0%, #2563eb 100%);
            border: none;
            color: #ffffff;
            font-size: 0.95rem;
            font-weight: 700;
            letter-spacing: 0.3px;
            box-shadow: 0 4px 12px rgba(37, 99, 235, 0.25);
            transition: all 0.2s ease;
            cursor: pointer;
        }
        .btn-submit-auth:hover {
            background: linear-gradient(135deg, #1d4ed8 0%, #1e40af 100%);
            box-shadow: 0 6px 18px rgba(37, 99, 235, 0.35);
            transform: translateY(-1px);
            color: #ffffff;
        }
        .btn-submit-auth:active {
            transform: translateY(0);
        }
        .login-footer-info {
            border-top: 1px solid #f1f5f9;
            padding-top: 20px;
            margin-top: 24px;
            text-align: center;
        }
        .mobile-brand-header {
            display: none;
            text-align: center;
            padding: 24px 20px 10px 20px;
            border-bottom: 1px solid #f1f5f9;
            background: #f8fafc;
        }
        .mobile-brand-header img {
            max-width: 170px;
            height: auto;
            margin-bottom: 10px;
        }
        @media (max-width: 991.98px) {
            .login-brand-panel {
                display: none !important;
            }
            .mobile-brand-header {
                display: block;
            }
            .login-wrapper {
                max-width: 480px;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-wrapper">
            <div class="login-card">
                <div class="row no-gutters">

                    <!-- Left Branding Column (Desktop) -->
                    <div class="col-lg-5 login-brand-panel">
                        <img src="Static/Images/newlrdelogo.png" alt="LRDE Logo" class="brand-logo-img" onerror="this.style.display='none'" />
                        <h2 class="brand-title">EMPLOYEE EDUCATION PORTAL</h2>
                        <p class="brand-subtitle">
                            Official Internal Academic &amp; Qualification Record Management System
                        </p>
                        <div class="badge-auth-security">
                            <i class="fas fa-shield-alt mr-2"></i> Official Authentication Portal
                        </div>
                    </div>

                    <!-- Right Form Column -->
                    <div class="col-lg-7">
                        <!-- Mobile / Tablet Brand Header -->
                        <div class="mobile-brand-header">
                            <img src="Static/Images/newlrdelogo.png" alt="LRDE Logo" onerror="this.style.display='none'" />
                            <h4 class="brand-title" style="font-size: 1.15rem;">EMPLOYEE EDUCATION PORTAL</h4>
                            <p class="brand-subtitle mb-0">Official Academic Records Portal</p>
                        </div>

                        <div class="login-form-panel">
                            <div class="mb-2">
                                <h1 class="form-title">Portal Login</h1>
                                <p class="form-subtitle">Sign in using your Employee PCNO and Password</p>
                            </div>

                            <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger py-2 px-3 small mb-3">
                                <div class="d-flex align-items-center">
                                    <i class="fas fa-exclamation-circle mr-2" style="font-size: 1.1rem;"></i>
                                    <div>
                                        <asp:Literal ID="litErrorMsg" runat="server"></asp:Literal>
                                    </div>
                                </div>
                            </asp:Panel>

                            <div class="form-group mb-3">
                                <label class="form-label-bold">Employee PCNO / Username <span class="req">*</span></label>
                                <div class="input-group-auth d-flex align-items-center">
                                    <span class="input-group-text"><i class="fas fa-id-badge"></i></span>
                                    <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="Enter your PCNO or Username" autocomplete="username"></asp:TextBox>
                                </div>
                            </div>

                            <div class="form-group mb-4">
                                <label class="form-label-bold">Password <span class="req">*</span></label>
                                <div class="input-group-auth d-flex align-items-center">
                                    <span class="input-group-text"><i class="fas fa-lock"></i></span>
                                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Enter your password" autocomplete="current-password"></asp:TextBox>
                                    <button type="button" class="btn-toggle-pwd" id="btnTogglePassword" title="Show / Hide Password" tabindex="-1">
                                        <i class="fas fa-eye" id="pwdEyeIcon"></i>
                                    </button>
                                </div>
                            </div>

                            <asp:Button ID="btnLogin" runat="server" Text="Sign In to Portal" CssClass="btn-submit-auth w-100 mb-3" OnClick="btnLogin_Click" />

                            <div class="login-footer-info">
                                <div class="d-flex align-items-center justify-content-center text-muted small mb-1">
                                    <i class="fas fa-lock mr-1 text-success"></i>
                                    <span>Authorized Personnel Only</span>
                                </div>
                                <div class="text-muted" style="font-size: 0.78rem;">
                                    For access issues, contact IT Helpdesk (ext. 2222 / itisg@lrde.com)
                                </div>
                            </div>
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </form>

    <!-- Local Offline jQuery Script -->
    <script src="Static/jquery/jquery.min.js"></script>
    <script>
        $(document).ready(function () {
            // Password reveal toggle
            $('#btnTogglePassword').on('click', function () {
                var $pwd = $('#<%= txtPassword.ClientID %>');
                var $icon = $('#pwdEyeIcon');
                if ($pwd.attr('type') === 'password') {
                    $pwd.attr('type', 'text');
                    $icon.removeClass('fa-eye').addClass('fa-eye-slash');
                } else {
                    $pwd.attr('type', 'password');
                    $icon.removeClass('fa-eye-slash').addClass('fa-eye');
                }
            });
        });
    </script>
</body>
</html>
