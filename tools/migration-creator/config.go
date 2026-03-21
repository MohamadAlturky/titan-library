package main

import (
	"encoding/json"
	"os"
)

const defaultMigrationsPath = `C:\Users\ASUS\RiderProjects\Titan.Library\Titan.Library.Infrastructure\Migrations`

type Config struct {
	MigrationsPath string `json:"migrations_path"`
}

func loadConfig() Config {
	data, err := os.ReadFile("config.json")
	if err != nil {
		return Config{MigrationsPath: defaultMigrationsPath}
	}
	var cfg Config
	if err := json.Unmarshal(data, &cfg); err != nil || cfg.MigrationsPath == "" {
		return Config{MigrationsPath: defaultMigrationsPath}
	}
	return cfg
}
