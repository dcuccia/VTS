%% Monte Carlo Demo
% This script includes an example that is the equivalent to 
% 1) Example 7 in vts_mc_demo.m: use N=1000 in infile_pMC_db_gen_template.txt
% 2) Example Example ROfRho (inverse solution for chromophore concentrations 
%   for multiple wavelengths, single rho) in vts_solver_demo.m: use 
%   wv=500:100:1000 and rho=1, N=10000
%   in infile_pMC_db_gen_template.txt
% but does not require MATLAB interop code to run
%%
clear all
clc
%% ======================================================================= %
% Example 1: Inverse solution for R(rho) = Example 7 in vts_mc_demo.m
% Run a Monte Carlo simulation with pMC post-processing enabled
% Use generated database to solve inverse problem with measured data
% generated using Nurbs
% NOTE: convergence to measured data optical properties affected by:
% 1) number of photons launched in baseline simulation, N
% 2) placement and number of rho
% 3) distance of initial guess from actual
% 4) normalization of chi2
% 5) optimset options selected
%% read in baseline OPs from database gen infile
x0 = [0.01, 5.0]; % baseline values for database and initial guess [mua, mus]
g = 0.8;
% input rho: in inversion don't use last point since includes tallies from
% beyond
rhostart=0.0;
rhostop=6.0;
rhocount=7;
rho=linspace(rhostart,rhostop,rhocount);
rhoMidpoints=(rho(1:end-1) + rho(2:end))/2;
infile_pMC='infile_pMC_db_gen.txt';
[status]=system(sprintf('cp infile_pMC_db_gen_template.txt %s',infile_pMC));
[status]=system(sprintf('./sub_ops.sh var1 %s %s','rho',infile_pMC));
[status]=system(sprintf('./sub_ops.sh a1 %f %s',x0(1),infile_pMC));
[status]=system(sprintf('./sub_ops.sh s1 %f %s',x0(2),infile_pMC));
[status]=system(sprintf('./sub_ops.sh sp1 %f %s',x0(2)*(1-g),infile_pMC));
[status]=system(sprintf('./sub_ops.sh rhostart %f %s',rhostart,infile_pMC));
[status]=system(sprintf('./sub_ops.sh rhostop %f %s',rhostop,infile_pMC));
[status]=system(sprintf('./sub_ops.sh rhocount %d %s',rhocount,infile_pMC));
% generate database
system('./mc infile=infile_pMC_db_gen.txt');
[R,pmcR,dmcRmua,dmcRmus]=load_for_inv_results('pMC_db_rho');
R_ig=R(1:end-1)';
%% use unconstrained optimization lb=[-inf -inf]; ub=[inf inf];
lb=[]; ub=[];

% input measData
measParms = [ 0.04 0.95 ];  % mua, musp
measData = [0.0331 0.0099 0.0042 0.0020 0.0010];

% option: divide measured data and forward model by measured data
% this counters log decay of data and relative importance of small rho data
% NOTE: if use option here, need to use option in pmc_F_dmc_J.m 
% measDataNorm = measData./measData;

% run lsqcurvefit if have Optimization Toolbox because it makes use of
% dMC differential Monte Carlo predictions
% if don't have Optimization Toolbox, run non-gradient, non-constrained
% fminsearch
if(exist('lsqcurvefit','file'))
    options = optimset('Jacobian','on','diagnostics','on','largescale','on');
    [recoveredOPs,resnorm] = lsqcurvefit('pmc_F_dmc_J_ex1',x0,rhoMidpoints,measData,lb,ub,...
        options,measData);
else
    %options = [];  % could try fminsearch but this code not tested
    %recoveredOPs = fminsearch('pmc_F_dmc_J_ex',x0,options,rhoMidpoints,measData);
end
[R,pmcR,dmcRmua,dmcRmus]=load_for_inv_results('PP_rho');
R_conv=pmcR(1:end-1)';
f = figure; semilogy(rhoMidpoints(1:end-1),measData,'r.',...
    rhoMidpoints(1:end-1),R_ig,'g-',...
    rhoMidpoints(1:end-1),R_conv,'b:','LineWidth',2);
xlabel('\rho [mm]');
ylabel('log10(R(\rho))');
legend('Meas','IG','Converged','Location','Best');
title('Inverse solution using pMC/dMC'); 
set(f, 'Name', 'Inverse solution using pMC/dMC');
disp(sprintf('IG   =    [%f %5.3f]',measParms(1),measParms(2)));
disp(sprintf('IG   =    [%f %5.3f] Chi2=%5.3e',x0(1),x0(2),...
    (measData-R_ig)*(measData-R_ig)'));
disp(sprintf('Conv =    [%f %5.3f] Chi2=%5.3e',recoveredOPs(1),recoveredOPs(2),...
    (measData-R_conv)*(measData-R_conv)'));
disp(sprintf('error=    [%f %5.3f]',abs(measParms(1)-recoveredOPs(1))/measParms(1),...
    abs(measParms(2)-recoveredOPs(2))/measParms(2)));

%% ======================================================================= %
% Example 2: Inverse solution for R(rho,wavelength) chromophore concentrations
% In vts_solver_demo: Example ROfRho (inverse solution for chromophore concentrations 
%  for multiple wavelengths, single rho) except only using every 100nm increments
% Run a Monte Carlo simulation with pMC post-processing enabled
% Use generated database to solve inverse problem with measured data
% generated using Nurbs and selected concentrations
rhostart=0;
rhostop=2;
rhocount=8;  
rho=linspace(rhostart,rhostop,rhocount);
rhoMidpoints=(rho(1:end-1) + rho(2:end))/2;
gen_db=true;
wv = 500:100:1000; % change from vts_solver_demo

% create a list of chromophore absorbers and their concentrations
% these values are the initial guess 
absorbers.Names =           {'HbO2', 'Hb', 'H2O'};
igConc =                    [ 70,    30,    0.8  ];
absorbers.Concentrations =  [igConc(1), igConc(2), igConc(3)];

% create a scatterer for power law
scatterers.Names = {}; % set empty if not fitting
scatterers.Coefficients = [1.2, 1.42];
g=0.8;
n=1.4;

% ops has dimensions [numwv 4]
[ops,dmua,dmusp]=get_optical_properties(absorbers,scatterers,wv); 

R_ig=zeros(1,length(wv));
infile_pMC='infile_pMC_db_gen.txt';
if (gen_db)
  for iwv=1:length(wv)
    [status]=system(sprintf('cp infile_pMC_db_gen_template.txt %s',infile_pMC));
    [status]=system(sprintf('./sub_ops.sh var1 %s %s',sprintf('wv%d',iwv),infile_pMC));
    [status]=system(sprintf('./sub_ops.sh a1 %f %s',ops(iwv,1),infile_pMC));
    [status]=system(sprintf('./sub_ops.sh s1 %f %s',ops(iwv,2)/(1-g),infile_pMC));
    [status]=system(sprintf('./sub_ops.sh sp1 %f %s',ops(iwv,2),infile_pMC));  
    [status]=system(sprintf('./sub_ops.sh rhostart %f %s',rhostart,infile_pMC));
    [status]=system(sprintf('./sub_ops.sh rhostop %f %s',rhostop,infile_pMC));
    [status]=system(sprintf('./sub_ops.sh rhocount %d %s',rhocount,infile_pMC));
    % generate databases for each wavelength
    system('./mc infile=infile_pMC_db_gen.txt');
  end
end
for iwv=1:length(wv)
  [R,pmcR,dmcRmua,dmcRmus]=load_for_inv_results(sprintf('pMC_db_wv%d',iwv));
  R_ig(iwv)=R(4);
end

%% use unconstrained optimization lb=[-inf -inf]; ub=[inf inf];
lb=[]; ub=[];
% input measData taken from vts_solver_demo using Nurbs rho=1mm and
% concentrations { 70, 30, 0.8 }
%measData = [0.0089 0.0221 0.0346 0.0301 0.0251 0.0198]; 
measParms = [ 72, 35, 1.2 ];
measData = [0.0082 0.0208 0.0342 0.0297 0.0246 0.0184];
% concentrations {80, 45, 3.5}
%measData = [0.0066 0.0183 0.0331 0.0285 0.0226 0.0131];
% run lsqcurvefit if have Optimization Toolbox because it makes use of
% dMC differential Monte Carlo predictions
% if don't have Optimization Toolbox, run non-gradient, non-constrained
% fminsearch
if(exist('lsqcurvefit','file'))
    options = optimoptions('lsqcurvefit','Algorithm','levenberg-marquardt',...
        'SpecifyObjectiveGradient',true,'Diagnostics','on');
    [recoveredOPs,resnorm] = lsqcurvefit('pmc_F_dmc_J_ex2',igConc,wv,measData',lb,ub,...
        options,rhoMidpoints,absorbers,scatterers,g,n);
else
    options = optimset('diagnostics','on','largescale','on');
    recoveredOPs = fminsearch('pmc_F_dmc_J_ex2',igConc,wv,options,rhoMidpoints,measData);
end
R_conv=zeros(1,length(wv));
for iwv=1:length(wv)
  [R,pmcR,dmcRmua,dmcRmus]=load_for_inv_results(sprintf('PP_wv%d',iwv));
  R_conv(iwv)=pmcR(4);
end
f = figure; plot(wv,measData,'r.',...
    wv,R_ig,'g-',...
    wv,R_conv,'b:','LineWidth',2);
xlabel('\lambda [nm]');
ylabel('log10(R(\lambda))');
legend('Meas','IG','Converged','Location','Best');
title('Inverse solution using pMC/dMC'); 
set(f, 'Name', 'Inverse solution using pMC/dMC');
disp(sprintf('Meas =    [%5.3f %5.3f %5.3f]',measParms(1),measParms(2),measParms(3)));
disp(sprintf('IG   =    [%5.3f %5.3f %5.3f] Chi2=%5.3e',igConc(1),igConc(2),...
    igConc(3),(measData-R_ig)*(measData-R_ig)'));
disp(sprintf('Conv =    [%5.3f %5.3f %5.3f] Chi2=%5.3e',recoveredOPs(1),recoveredOPs(2),...
    recoveredOPs(3),(measData-R_conv)*(measData-R_conv)'));
disp(sprintf('error=    [%5.3f %5.3f %5.3f]',abs(measParms(1)-recoveredOPs(1))/measParms(1),...
    abs(measParms(2)-recoveredOPs(2))/measParms(2),abs(measParms(3)-recoveredOPs(3))/measParms(3)));
%% ======================================================================= %
% Example 3: Inverse solution for R(rho,wavelength) chromophore concentrations
%   and scatterer coefficients
rhostart=0;
rhostop=2;
rhocount=8;  
rho=linspace(rhostart,rhostop,rhocount);
rhoMidpoints=(rho(1:end-1) + rho(2:end))/2;
numrho=2;
gen_db=true;
wv = 600:100:900; % change from vts_solver_demo

% create a list of chromophore absorbers and their concentrations
% these values are the EXACT solution
absorbers.Names =           {'Hb','HbO2'};
absorbers.Concentrations =  [ 18.0, 30.0 ];

% create a scatterer for power law
scatterers.Names = {'a','b'};
scatterers.Coefficients(1) = 0.8;
scatterers.Coefficients(2) = 1.6;
g=0.8;
n=1.4;
igParms = [absorbers.Concentrations(1) absorbers.Concentrations(2) scatterers.Coefficients(1) scatterers.Coefficients(2)];

% ops has dimensions [numwv 4]
[ops,dmua,dmusp]=get_optical_properties(absorbers,scatterers,wv); 

infile_pMC='infile_pMC_db_gen.txt';
if (gen_db)
  for iwv=1:length(wv)
    [status]=system(sprintf('cp infile_pMC_db_gen_template.txt %s',infile_pMC));
    [status]=system(sprintf('./sub_ops.sh var1 %s %s',sprintf('wv%d',iwv),infile_pMC));
    [status]=system(sprintf('./sub_ops.sh a1 %f %s',ops(iwv,1),infile_pMC));
    [status]=system(sprintf('./sub_ops.sh s1 %f %s',ops(iwv,2)/(1-g),infile_pMC));
    [status]=system(sprintf('./sub_ops.sh sp1 %f %s',ops(iwv,2),infile_pMC));  
    [status]=system(sprintf('./sub_ops.sh rhostart %f %s',rhostart,infile_pMC));
    [status]=system(sprintf('./sub_ops.sh rhostop %f %s',rhostop,infile_pMC));
    [status]=system(sprintf('./sub_ops.sh rhocount %d %s',rhocount,infile_pMC));
    % generate databases for each wavelength
    system('./mc infile=infile_pMC_db_gen.txt');
  end
end
R_ig=zeros(1,length(wv)*numrho);

for iwv=1:length(wv)
  [R,pmcR,dmcRmua,dmcRmus]=load_for_inv_results(sprintf('pMC_db_wv%d',iwv));
  R_ig(iwv)=R(1);
  R_ig(iwv+length(wv))=R(4);
end

%% use unconstrained optimization lb=[-inf -inf]; ub=[inf inf];
lb=[]; ub=[];
% input measData taken from vts_solver_demo using Nurbs rhoMidpoint=0.1429,1.0mm and
% parameters { 28.4, 22.4, 1.2, 1.42 }
measParms = [28.4, 22.4, 1.2, 1.42 ];
measData = [0.2931 0.2548 0.2068 0.1711 0.0255 0.0355 0.0320 0.0277]; 
% run lsqcurvefit if have Optimization Toolbox because it makes use of
% dMC differential Monte Carlo predictions
% if don't have Optimization Toolbox, run non-gradient, non-constrained
% fminsearch
if(exist('lsqcurvefit','file'))
    options = optimoptions('lsqcurvefit','Algorithm','levenberg-marquardt',...
        'SpecifyObjectiveGradient',true,'Diagnostics','on');
    [recoveredOPs,resnorm] = lsqcurvefit('pmc_F_dmc_J_ex3',igParms,[wv wv],measData',lb,ub,...
        options,rhoMidpoints,absorbers,scatterers,g,n);
else
    options = optimset('diagnostics','on','largescale','on');
    recoveredOPs = fminsearch('pmc_F_dmc_J_wv',igParms,wv,options,rhoMidpoints,measData);
end
R_conv=zeros(1,length(wv));
for iwv=1:length(wv)
  [R,pmcR,dmcRmua,dmcRmus]=load_for_inv_results(sprintf('PP_wv%d',iwv));
  R_conv(iwv)=pmcR(1);
  R_conv(iwv+length(wv))=pmcR(4);
end
f = figure; plot(wv,measData(1:length(wv)),'rx',wv,measData(length(wv)+1:end),'ro',...
    wv,R_ig(1:length(wv)),'g-',wv,R_ig(length(wv)+1:end),'g--',...
    wv,R_conv(1:length(wv)),'b-',wv,R_conv(length(wv)+1:end),'b--','LineWidth',2);
xlabel('\lambda [nm]');
ylabel('log10(R(\lambda))');
legend('Meas rho=0.1429','Meas rho=1.0','IG rho=0.1429','IG rho=1.0',...
    'Converged rho=0.1429','Converged rho=1.0','Location','Best');
title('Inverse solution using pMC/dMC'); 
set(f, 'Name', 'Inverse solution using pMC/dMC');
disp(sprintf('Meas =    [%5.3f %5.3f %5.3f %5.3f]',measParms(1),measParms(2),...
    measParms(3),measParms(4)));
disp(sprintf('IG   =    [%5.3f %5.3f %5.3f %5.3f] Chi2=%5.3e',igParms(1),igParms(2),...
    igParms(3),igParms(4),(measData-R_ig)*(measData-R_ig)'));
disp(sprintf('Conv =    [%5.3f %5.3f %5.3f %5.3f] Chi2=%5.3e',recoveredOPs(1),recoveredOPs(2),...
    recoveredOPs(3),recoveredOPs(4),(measData-R_conv)*(measData-R_conv)'));
disp(sprintf('error=    [%5.3f %5.3f %5.3f %5.3f]',abs(measParms(1)-recoveredOPs(1))/measParms(1),...
    abs(measParms(2)-recoveredOPs(2))/measParms(2),abs(measParms(3)-recoveredOPs(3))/measParms(3),...
    abs(measParms(4)-recoveredOPs(4))/measParms(4)));

