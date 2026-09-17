import React from 'react';
import { Card, CardContent, CardActions, Typography, Box, Chip, IconButton } from '@mui/material';
import { ChevronRight } from '@mui/icons-material';

interface MobileCardProps {
  title: string;
  subtitle?: string;
  primaryValue?: string;
  secondaryValue?: string;
  status?: string;
  statusColor?: 'success' | 'warning' | 'error' | 'info' | 'default';
  icon?: React.ReactNode;
  onClick?: () => void;
  action?: React.ReactNode;
}

const MobileCard: React.FC<MobileCardProps> = ({
  title,
  subtitle,
  primaryValue,
  secondaryValue,
  status,
  statusColor = 'default',
  icon,
  onClick,
  action,
}) => {
  return (
    <Card
      onClick={onClick}
      sx={{
        mb: 1.5,
        cursor: onClick ? 'pointer' : 'default',
        '&:hover': onClick ? { boxShadow: 2 } : {},
        borderRadius: 2,
      }}
    >
      <CardContent sx={{ pb: action ? 1 : 2 }}>
        <Box sx={{ display: 'flex', alignItems: 'flex-start', justifyContent: 'space-between' }}>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5, flex: 1 }}>
            {icon && (
              <Box sx={{ color: 'primary.main', display: 'flex', alignItems: 'center' }}>
                {icon}
              </Box>
            )}
            <Box sx={{ flex: 1, minWidth: 0 }}>
              <Typography variant="subtitle1" fontWeight="bold" noWrap>
                {title}
              </Typography>
              {subtitle && (
                <Typography variant="body2" color="text.secondary" noWrap>
                  {subtitle}
                </Typography>
              )}
            </Box>
          </Box>
          <Box sx={{ textAlign: 'right', display: 'flex', alignItems: 'center', gap: 1 }}>
            {status && (
              <Chip label={status} size="small" color={statusColor} variant="outlined" />
            )}
            {onClick && <ChevronRight color="action" />}
          </Box>
        </Box>
        {(primaryValue || secondaryValue) && (
          <Box sx={{ mt: 1, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            {primaryValue && (
              <Typography variant="h6" fontWeight="bold" color="primary">
                {primaryValue}
              </Typography>
            )}
            {secondaryValue && (
              <Typography variant="body2" color="text.secondary">
                {secondaryValue}
              </Typography>
            )}
          </Box>
        )}
      </CardContent>
      {action && (
        <CardActions sx={{ pt: 0, px: 2, pb: 1.5 }}>
          {action}
        </CardActions>
      )}
    </Card>
  );
};

export default MobileCard;
