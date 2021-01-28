namespace Capgemini.PowerApps.PackageDeployerTemplate.Config
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// The configuration used by <see cref="PackageTemplateBase"/>.
    /// </summary>
    public class TemplateConfig
    {
        /// <summary>
        /// Gets or sets a collection of processes and their deployment configuration.
        /// </summary>
        /// <value>
        /// A collection of processes and their deployment configuration.
        /// </value>
        [XmlArray("processes")]
        [XmlArrayItem("process")]
        public ProcessConfig[] Processes { get; set; } = Array.Empty<ProcessConfig>();

        /// <summary>
        /// Gets processes configured to be inactive after deployment.
        /// </summary>
        /// <returns>Processes configured to be inactive after deployment.</returns>
        [XmlIgnore]
        public IEnumerable<ProcessConfig> ProcessesToDeactivate => this.Processes.Where(p => p.DesiredState == DesiredState.Inactive);

        /// <summary>
        /// Gets processes configured to be active after deployment.
        /// </summary>
        /// <returns>Processes configured to be active after deployment.</returns>
        [XmlIgnore]
        public IEnumerable<ProcessConfig> ProcessesToActivate => this.Processes.Where(p => p.DesiredState == DesiredState.Active);

        /// <summary>
        /// Gets or sets a collection of SDK steps and their deployment configuration.
        /// </summary>
        /// <value>
        /// A collection of SDK steps and their deployment configuration.
        /// </value>
        [XmlArray("sdksteps")]
        [XmlArrayItem("sdkstep")]
        public SdkStepConfig[] SdkSteps { get; set; } = Array.Empty<SdkStepConfig>();

        /// <summary>
        /// Gets SDK steps configured to be inactive after deployment.
        /// </summary>
        /// <returns>SDK steps configured to be inactive after deployment.</returns>
        [XmlIgnore]
        public IEnumerable<SdkStepConfig> SdkStepsToDeactivate => this.SdkSteps.Where(p => p.DesiredState == DesiredState.Inactive);

        /// <summary>
        /// Gets SDK steps configured to be active after deployment.
        /// </summary>
        /// <returns>SDK steps configured to be active after deployment.</returns>
        [XmlIgnore]
        public IEnumerable<SdkStepConfig> SdkStepsToActivate => this.SdkSteps.Where(p => p.DesiredState == DesiredState.Active);

        /// <summary>
        /// Gets or sets a collection of word templates and their deployment configuration.
        /// </summary>
        /// <value>
        /// A collection of word templates and their deployment configuration.
        /// </value>
        [XmlArray("documenttemplates")]
        [XmlArrayItem("documenttemplate")]
        public DocumentTemplateConfig[] DocumentTemplates { get; set; } = Array.Empty<DocumentTemplateConfig>();

        /// <summary>
        /// Gets or sets a list of data import configurations.
        /// Refer to https://github.com/Capgemini/xrm-datamigration for more info.
        /// </summary>
        /// <value>
        /// A list of data import configurations.
        /// </value>
        [XmlArray("dataimports")]
        [XmlArrayItem("dataimport")]
        public DataImportConfig[] DataImports { get; set; } = Array.Empty<DataImportConfig>();

        /// <summary>
        /// Gets a collection of data import configurations to run post-deployment.
        /// </summary>
        /// <returns>A collection of data import configurations to run post-deployment.</returns>
        [XmlIgnore]
        public IEnumerable<DataImportConfig> PostDeployDataImports => this.DataImports.Where(c => !c.ImportBeforeSolutions);

        /// <summary>
        /// Gets a collection of data import configurations to run pre-deployment.
        /// </summary>
        /// <returns>A collection of data import configurations to run post-deployment.</returns>
        [XmlIgnore]
        public IEnumerable<DataImportConfig> PreDeployDataImports => this.DataImports.Where(c => c.ImportBeforeSolutions);

        /// <summary>
        /// Gets or sets a collection of SLAs and their deployment configuration.
        /// </summary>
        /// <value>
        /// A collection of SLAs and their deployment configuration.
        /// </value>
        [XmlArray("slas")]
        [XmlArrayItem("sla")]
        public SlaConfig[] Slas { get; set; } = Array.Empty<SlaConfig>();

        /// <summary>
        /// Gets SLAs configured to be set as default.
        /// </summary>
        /// <value>
        /// SLAs configured to be set as default.
        /// </value>
        [XmlIgnore]
        public IEnumerable<SlaConfig> DefaultSlas => this.Slas.Where(sla => sla.IsDefault);

        /// <summary>
        /// Gets or sets a value indicating whether to activate/deactivate SLAs before/after deployment.
        /// </summary>
        /// <value>
        /// A value indicating whether to activate/deactivate SLAs before/after deployment.
        /// </value>
        [XmlAttribute("activatedeactivateslas")]
        public bool ActivateDeactivateSLAs { get; set; } = true;

        /// <summary>
        /// Load the template config from the specified path.
        /// </summary>
        /// <param name="importConfigPath">The path of the import config file.</param>
        /// <returns>The template config.</returns>
        public static TemplateConfig Load(string importConfigPath)
        {
            using var fs = new FileStream(importConfigPath.ToString(), FileMode.Open);
            return ((ConfigDataStorage)new XmlSerializer(typeof(ConfigDataStorage)).Deserialize(fs)).TemplateConfig;
        }
    }
}