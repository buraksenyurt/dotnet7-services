use axum::response::IntoResponse;
use axum::routing::get;
use axum::{Json, Router};
use chrono::Utc;
use rand::Rng;
use serde::Serialize;
use std::net::SocketAddr;

#[tokio::main]
async fn main() {
    let route = Router::new().route("/report", get(default_handler));
    let app = Router::new().nest("/api", route);

    let address = SocketAddr::from(([127, 0, 0, 1], 6023));
    println!(
        "Server online: {}.\nReporting last measurements...",
        address
    );

    axum::Server::bind(&address)
        .serve(app.into_make_service())
        .await
        .expect("Failed to start server");
}

async fn default_handler() -> impl IntoResponse {
    let mut rng = rand::thread_rng();
    let app = Artifact::new(
        "RockShop Web".to_string(),
        rng.gen_range(0.0..20.0) as f32,
        rng.gen_range(0.0..20.0) as f32,
        rng.gen_range(0.0..20.0) as f32,
    );
    Json(app)
}

#[derive(Serialize)]
struct Artifact {
    #[serde(rename = "ApplicationName")]
    pub name: String,
    #[serde(rename = "Time")]
    pub time: String,
    #[serde(rename = "CodeCoverage")]
    pub code_coverage: f32,
    #[serde(rename = "CognitiveComplexity")]
    pub cognitive_complexity: f32,
    #[serde(rename = "DuplicatedCode")]
    pub duplicated_code: f32,
}

impl Artifact {
    pub fn new(
        name: String,
        code_coverage: f32,
        cognitive_complexity: f32,
        duplicated_code: f32,
    ) -> Self {
        Self {
            name,
            time: Utc::now().to_string(),
            code_coverage,
            cognitive_complexity,
            duplicated_code,
        }
    }
}
