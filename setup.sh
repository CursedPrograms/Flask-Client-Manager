#!/bin/bash

# This is a setup script for the Python project

echo "Setting up the Python environment..."

# Install dependencies
if command -v pip &> /dev/null; then
    echo "Installing Python dependencies..."
    pip install -r requirements.txt
else
    echo "Error: pip not found. Please install Python and pip first."
    exit 1
fi

# Run the app Python script
echo "Running app.py..."
python app.py

echo "Setup complete! You're ready to start working on your Python project."
