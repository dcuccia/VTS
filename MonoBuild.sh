SolutionDir=$PWD

#Bring in latest libraries using NuGet - no longer necessary
#./GetMonoLibs.sh 

# tell mono certs to trust when doing https:
# following is version for ubuntu or debian dists else RedHat dist
if [ -f /etc/lsb-release ]; then
  sudo cert-sync /etc/ssl/certs/ca-certificates.crt
elif [ -f /etc/redhat-release ]; then
  sudo cert-sync /etc/pki/tls/certs/ca-bundle.crt
fi

# use an environment variable to enable package restore 
export EnableNuGetPackageRestore=true

#Build the solution in Debug configuration
msbuild $SolutionDir/src/Vts-Mono.sln /p:WarningLevel=2

#Build the solution in Release configuration
msbuild $SolutionDir/src/Vts-Mono.sln /p:configuration=Release /p:WarningLevel=2

#Post build events:
ConfigName=Debug
Source=$SolutionDir/src/Vts.Desktop/bin
Destination=$SolutionDir/build/core/Desktop

mkdir -p $Destination/$ConfigName 
yes | cp $Source/$ConfigName/* $Destination/$ConfigName

ConfigName=Release

mkdir -p $Destination/$ConfigName 
yes | cp $Source/$ConfigName/* $Destination/$ConfigName

ConfigName=Debug
Source=$SolutionDir/src/Vts.MonteCarlo.PostProcessor/bin
Destination=$SolutionDir/build/apps/mc_post

mkdir -p $Destination/$ConfigName 
yes | cp $Source/$ConfigName/* $Destination/$ConfigName

ConfigName=Release

mkdir -p $Destination/$ConfigName 
yes | cp $Source/$ConfigName/* $Destination/$ConfigName

ConfigName=Debug
Source=$SolutionDir/src/Vts.MonteCarlo.CommandLineApplication/bin
Destination=$SolutionDir/build/apps/mc

mkdir -p $Destination/$ConfigName 
yes | cp $Source/$ConfigName/* $Destination/$ConfigName

ConfigName=Release

mkdir -p $Destination/$ConfigName 
yes | cp $Source/$ConfigName/* $Destination/$ConfigName

#Generate the XML input files
cd $Destination/$ConfigName
mono mc.exe geninfiles
