<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ongc_webapp._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .welcome-hero-section {
            padding: 60px 0;
            background: #ffffff;
        }

        .hero-title {
            color: #2D3748;
            font-weight: 800;
            font-size: 2.5rem;
            letter-spacing: -1px;
            margin-bottom: 15px;
        }

        .hero-subtitle {
            color: #718096;
            font-size: 1.15rem;
            line-height: 1.7;
            max-width: 800px;
            margin-bottom: 40px;
        }

        .feature-card {
            border: 1px solid #E2E8F0;
            border-radius: 12px;
            padding: 30px;
            background-color: #ffffff;
            transition: all 0.3s ease;
            height: 100%;
        }

        .feature-card:hover {
            transform: translateY(-4px);
            box-shadow: 0 12px 24px rgba(0,0,0,0.06);
            border-color: #8a0414;
        }

        .feature-icon-title {
            color: #8a0414;
            font-weight: 700;
            font-size: 1.25rem;
            margin-bottom: 12px;
        }

        .feature-text {
            color: #4A5568;
            font-size: 0.95rem;
            line-height: 1.6;
        }
    </style>

    <div class="welcome-hero-section">
        <div class="row align-items-center mb-5">
            <div class="col-12">
                <h1 class="hero-title">Welcome to the ONGC Foundation Portal</h1>
                <p class="hero-subtitle">
                    Empowering communities and driving meaningful socio-economic development across the nation. Securely access internal document repositories, operational templates, and indexing matrices below.
                </p>
            </div>
        </div>

        <div class="row g-4">
            <div class="col-md-4">
                <div class="feature-card">
                    <h3 class="feature-icon-title">Secure Repository</h3>
                    <p class="feature-text">
                        Access official project documentation, trust logs, and regulatory compliance assets safely managed under centralized structural schemas.
                    </p>
                </div>
            </div>
            <div class="col-md-4">
                <div class="feature-card">
                    <h3 class="feature-icon-title">Metadata Indexing</h3>
                    <p class="feature-text">
                        Seamlessly categorize and archive legal frameworks, allocation charts, and project spreadsheets using high-performance relational mapping templates.
                    </p>
                </div>
            </div>
            <div class="col-md-4">
                <div class="feature-card">
                    <h3 class="feature-icon-title">Analytical Reporting</h3>
                    <p class="feature-text">
                        Generate granular performance summaries and allocation matrices mapped to key regional trust development goals.
                    </p>
                </div>
            </div>
        </div>
    </div>
</asp:Content>