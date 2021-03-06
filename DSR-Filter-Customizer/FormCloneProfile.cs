﻿using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DSR_Filter_Customizer
{
    public partial class FormCloneProfile : Form
    {
        public FilterProfile Result = null;
        private FilterProfile clone;

        public FormCloneProfile(FilterProfile clone)
        {
            InitializeComponent();
            this.clone = clone;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            Result = clone.Clone(txtName.Text);
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            if (txtName.TextLength > 0 && Regex.IsMatch(txtName.Text, @"[\w\d]"))
                btnConfirm.Enabled = true;
            else
                btnConfirm.Enabled = false;
        }
    }
}
