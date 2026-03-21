package main

import (
	"fmt"
	"strings"

	"github.com/charmbracelet/bubbles/textinput"
	tea "github.com/charmbracelet/bubbletea"
	"github.com/charmbracelet/lipgloss"
)

type state int

const (
	stateInput   state = iota
	statePreview state = iota
	stateDone    state = iota
)

var (
	titleStyle   = lipgloss.NewStyle().Bold(true).Foreground(lipgloss.Color("205"))
	labelStyle   = lipgloss.NewStyle().Foreground(lipgloss.Color("241"))
	valueStyle   = lipgloss.NewStyle().Foreground(lipgloss.Color("86"))
	errorStyle   = lipgloss.NewStyle().Foreground(lipgloss.Color("196"))
	successStyle = lipgloss.NewStyle().Foreground(lipgloss.Color("82")).Bold(true)
	dimStyle     = lipgloss.NewStyle().Foreground(lipgloss.Color("238"))
	hintStyle    = lipgloss.NewStyle().Foreground(lipgloss.Color("243")).Italic(true)
	boxStyle     = lipgloss.NewStyle().
			Border(lipgloss.RoundedBorder()).
			BorderForeground(lipgloss.Color("62")).
			Padding(0, 1)
)

type model struct {
	cfg       Config
	input     textinput.Model
	state     state
	maxOrder  int
	nextOrder int
	createdAt string
	errMsg    string
	scanErr   string
}

func newModel(cfg Config) model {
	ti := textinput.New()
	ti.Placeholder = "e.g. AddIndexToBooks"
	ti.Focus()
	ti.CharLimit = 100
	ti.Width = 50

	maxOrder, err := scanMaxOrder(cfg.MigrationsPath)
	scanErrMsg := ""
	if err != nil {
		scanErrMsg = err.Error()
	}

	return model{
		cfg:       cfg,
		input:     ti,
		state:     stateInput,
		maxOrder:  maxOrder,
		nextOrder: maxOrder + 1,
		scanErr:   scanErrMsg,
	}
}

func (m model) Init() tea.Cmd {
	return textinput.Blink
}

func (m model) Update(msg tea.Msg) (tea.Model, tea.Cmd) {
	switch msg := msg.(type) {
	case tea.KeyMsg:
		switch msg.Type {
		case tea.KeyCtrlC, tea.KeyEsc:
			return m, tea.Quit

		case tea.KeyEnter:
			switch m.state {
			case stateInput:
				name := strings.TrimSpace(m.input.Value())
				if name == "" {
					return m, nil
				}
				m.state = statePreview
				return m, nil

			case statePreview:
				name := strings.TrimSpace(m.input.Value())
				path, err := createMigration(m.cfg.MigrationsPath, name, m.nextOrder)
				if err != nil {
					m.errMsg = err.Error()
				} else {
					m.createdAt = path
				}
				m.state = stateDone
				return m, nil

			case stateDone:
				return m, tea.Quit
			}
		}
	}

	if m.state == stateInput {
		var cmd tea.Cmd
		m.input, cmd = m.input.Update(msg)
		return m, cmd
	}
	return m, nil
}

func (m model) View() string {
	var b strings.Builder

	b.WriteString(titleStyle.Render("Migration Creator") + "\n")
	b.WriteString(labelStyle.Render(fmt.Sprintf("Path: %s", m.cfg.MigrationsPath)) + "\n")
	if m.scanErr != "" {
		b.WriteString(errorStyle.Render("  Warning: "+m.scanErr) + "\n")
	}
	b.WriteString("\n")

	switch m.state {
	case stateInput:
		b.WriteString(labelStyle.Render("Migration name (without 'Migration.cs'):") + "\n")
		b.WriteString(m.input.View() + "\n\n")
		b.WriteString(hintStyle.Render(fmt.Sprintf("Detected max order: %d  →  next will be: %d", m.maxOrder, m.nextOrder)) + "\n\n")
		b.WriteString(dimStyle.Render("enter to continue • esc to quit"))

	case statePreview:
		name := strings.TrimSpace(m.input.Value())
		fileName := name + ".cs"
		buildBoilerplate(name, m.nextOrder)

		b.WriteString(labelStyle.Render("File to create: ") + valueStyle.Render(fileName) + "\n")
		b.WriteString(labelStyle.Render("Order:          ") + valueStyle.Render(fmt.Sprintf("%d", m.nextOrder)) + "\n\n")
		b.WriteString(labelStyle.Render("Done") + "\n")
		b.WriteString(hintStyle.Render("enter to create • esc to quit"))

	case stateDone:
		if m.errMsg != "" {
			b.WriteString(errorStyle.Render("Error: "+m.errMsg) + "\n\n")
		} else {
			b.WriteString(successStyle.Render("Created!") + "\n")
			b.WriteString(valueStyle.Render(m.createdAt) + "\n\n")
		}
		b.WriteString(dimStyle.Render("enter or esc to exit"))
	}

	return b.String()
}
