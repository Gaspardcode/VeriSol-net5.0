#! /bin/bash
DIR=$(dirname $0)

# If DOTNET_TOOLS is not set, default to the standard user-level tool directory.
# This makes the script runnable both locally and in the Docker container.
export DOTNET_TOOLS=${DOTNET_TOOLS:-"$HOME/.dotnet/tools"}
mkdir -p "$DOTNET_TOOLS"

# Use --tool-path to install to a specific directory.
# The uninstall commands are best-effort and may fail if the tools aren't present.
dotnet tool uninstall VeriSol --tool-path "$DOTNET_TOOLS" || true
dotnet tool uninstall SolToBoogieTest --tool-path "$DOTNET_TOOLS" || true

dotnet build --configuration Release $DIR/Sources/VeriSol.sln
dotnet tool install VeriSol --version 0.1.1-alpha --tool-path "$DOTNET_TOOLS" --add-source $DIR/Sources/nupkg/
dotnet tool install SolToBoogieTest --version 0.1.1-alpha --tool-path "$DOTNET_TOOLS" --add-source $DIR/Sources/nupkg/
